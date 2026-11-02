using Microsoft.EntityFrameworkCore;
using MavisBelleza.Data;
using MavisBelleza.Models;

namespace MavisBelleza.Services;

/// <summary>Filtros del listado de turnos.</summary>
public record FiltroTurnos(string? Texto = null, string? Estado = null, int Pagina = 1, int TamanioPagina = 6);

/// <summary>Se lanza cuando el horario elegido ya está tomado por otro turno activo.</summary>
public class HorarioOcupadoException : Exception
{
    public HorarioOcupadoException() : base("El horario seleccionado ya no está disponible para ese día.") { }
}

/// <summary>
/// Capa de acceso a datos de turnos. Cada método crea su propio DbContext con
/// IDbContextFactory (recomendado en Blazor Server porque un circuito vive mucho tiempo).
/// </summary>
public class TurnoService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly TurnoStateService _state;

    public TurnoService(IDbContextFactory<AppDbContext> dbFactory, TurnoStateService state)
    {
        _dbFactory = dbFactory;
        _state = state;
    }

    // ===================== LECTURA =====================

    public async Task<PagedResult<Turno>> BuscarAsync(FiltroTurnos filtro)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var query = AplicarFiltros(db, filtro);

        var total = await query.CountAsync();
        var tamanio = Math.Clamp(filtro.TamanioPagina, 1, 50);
        var totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamanio));
        var pagina = Math.Clamp(filtro.Pagina, 1, totalPaginas);

        var items = await query
            .OrderByDescending(t => t.Fecha >= DateTime.Today) // primero los próximos
            .ThenBy(t => t.Fecha)
            .ThenBy(t => t.Horario)
            .Skip((pagina - 1) * tamanio)
            .Take(tamanio)
            .ToListAsync();

        return new PagedResult<Turno> { Items = items, TotalItems = total, Pagina = pagina, TamanioPagina = tamanio };
    }

    /// <summary>Todos los turnos que cumplen el filtro (sin paginar), para exportar.</summary>
    public async Task<List<Turno>> ListarParaExportarAsync(FiltroTurnos filtro)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await AplicarFiltros(db, filtro)
            .OrderBy(t => t.Fecha).ThenBy(t => t.Horario)
            .ToListAsync();
    }

    public async Task<Turno?> ObtenerAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Turnos
            .AsNoTracking()
            .Include(t => t.Cliente)
            .Include(t => t.Servicio)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    /// <summary>Horarios ya tomados (no cancelados) en una fecha. Se puede excluir el turno que se está editando.</summary>
    public async Task<HashSet<string>> HorariosOcupadosAsync(DateTime fecha, int? excluirTurnoId = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var dia = fecha.Date;
        var horarios = await db.Turnos
            .Where(t => t.Fecha == dia && t.Estado != EstadosTurno.Cancelado)
            .Where(t => excluirTurnoId == null || t.Id != excluirTurnoId)
            .Select(t => t.Horario)
            .ToListAsync();
        return horarios.ToHashSet();
    }

    // ===================== ESCRITURA =====================

    /// <summary>
    /// Crea o actualiza un turno. Si ya existe un cliente con ese email lo reutiliza
    /// (y actualiza sus datos); si no, lo crea. Devuelve el Id del turno.
    /// </summary>
    public async Task<int> GuardarAsync(TurnoFormModel model)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        // Doble chequeo de disponibilidad (otro usuario pudo reservar mientras se completaba el form)
        var dia = model.Fecha.Date;
        bool ocupado = await db.Turnos.AnyAsync(t =>
            t.Fecha == dia && t.Horario == model.Horario &&
            t.Estado != EstadosTurno.Cancelado && t.Id != model.Id);
        if (ocupado && model.Estado != EstadosTurno.Cancelado)
            throw new HorarioOcupadoException();

        var servicio = await db.Servicios.FindAsync(model.ServicioId)
                       ?? throw new InvalidOperationException("El servicio seleccionado no existe.");

        // Cliente: buscar por email o crear
        var email = model.Email.Trim().ToLowerInvariant();
        var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.Email == email);
        if (cliente is null)
        {
            cliente = new Cliente { Email = email };
            db.Clientes.Add(cliente);
        }
        cliente.Nombre = model.Nombre.Trim();
        cliente.Apellido = model.Apellido.Trim();
        cliente.Telefono = model.Telefono.Trim();

        Turno turno;
        if (model.EsEdicion)
        {
            turno = await db.Turnos.FindAsync(model.Id)
                    ?? throw new KeyNotFoundException($"No existe el turno {model.Id}.");
            if (turno.ServicioId != model.ServicioId)
                turno.Precio = servicio.Precio; // si cambia el servicio, se actualiza el precio
        }
        else
        {
            turno = new Turno { Precio = servicio.Precio, FechaCreacion = DateTime.Now };
            db.Turnos.Add(turno);
        }

        turno.Cliente = cliente;
        turno.ServicioId = model.ServicioId;
        turno.Fecha = dia;
        turno.Horario = model.Horario;
        turno.MetodoPago = model.MetodoPago;
        turno.Estado = model.Estado;
        turno.Notas = string.IsNullOrWhiteSpace(model.Notas) ? null : model.Notas.Trim();

        await db.SaveChangesAsync();
        await _state.NotificarCambiosAsync();
        return turno.Id;
    }

    public async Task CambiarEstadoAsync(int id, string nuevoEstado)
    {
        if (!EstadosTurno.Todos.Contains(nuevoEstado))
            throw new ArgumentException($"Estado inválido: {nuevoEstado}");

        await using var db = await _dbFactory.CreateDbContextAsync();
        var turno = await db.Turnos.FindAsync(id);
        if (turno is null) return;

        turno.Estado = nuevoEstado;
        await db.SaveChangesAsync();
        await _state.NotificarCambiosAsync();
    }

    public async Task<bool> EliminarAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var turno = await db.Turnos.FindAsync(id);
        if (turno is null) return false;

        db.Turnos.Remove(turno);
        await db.SaveChangesAsync();
        await _state.NotificarCambiosAsync();
        return true;
    }

    // ===================== HELPERS =====================

    private static IQueryable<Turno> AplicarFiltros(AppDbContext db, FiltroTurnos filtro)
    {
        IQueryable<Turno> query = db.Turnos
            .AsNoTracking()
            .Include(t => t.Cliente)
            .Include(t => t.Servicio);

        if (!string.IsNullOrWhiteSpace(filtro.Estado))
            query = query.Where(t => t.Estado == filtro.Estado);

        if (!string.IsNullOrWhiteSpace(filtro.Texto))
        {
            var patron = $"%{filtro.Texto.Trim()}%";
            query = query.Where(t =>
                EF.Functions.Like(t.Cliente!.Nombre, patron) ||
                EF.Functions.Like(t.Cliente!.Apellido, patron) ||
                EF.Functions.Like(t.Cliente!.Email, patron) ||
                EF.Functions.Like(t.Cliente!.Telefono, patron) ||
                EF.Functions.Like(t.Servicio!.Nombre, patron));
        }

        return query;
    }
}
