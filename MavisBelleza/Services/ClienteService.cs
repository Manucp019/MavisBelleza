using Microsoft.EntityFrameworkCore;
using MavisBelleza.Data;
using MavisBelleza.Models;

namespace MavisBelleza.Services;

/// <summary>Fila del listado de clientes (proyección liviana con datos calculados).</summary>
public record ClienteListado(int Id, string Nombre, string Apellido, string Telefono, string Email, int CantidadTurnos, DateTime? ProximoTurno)
{
    public string NombreCompleto => $"{Nombre} {Apellido}";
}

/// <summary>Resultado de una operación que puede fallar por una regla de negocio.</summary>
public record ResultadoOperacion(bool Exito, string? Error = null, int Id = 0)
{
    public static ResultadoOperacion Ok(int id = 0) => new(true, null, id);
    public static ResultadoOperacion Falla(string error) => new(false, error);
}

public class ClienteService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public ClienteService(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<PagedResult<ClienteListado>> BuscarAsync(string? texto, int pagina, int tamanioPagina = 8)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var hoy = DateTime.Today;

        IQueryable<Cliente> query = db.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var patron = $"%{texto.Trim()}%";
            query = query.Where(c =>
                EF.Functions.Like(c.Nombre, patron) ||
                EF.Functions.Like(c.Apellido, patron) ||
                EF.Functions.Like(c.Email, patron) ||
                EF.Functions.Like(c.Telefono, patron));
        }

        var total = await query.CountAsync();
        var tamanio = Math.Clamp(tamanioPagina, 1, 50);
        var totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamanio));
        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var items = await query
            .OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)
            .Skip((pagina - 1) * tamanio)
            .Take(tamanio)
            .Select(c => new ClienteListado(
                c.Id, c.Nombre, c.Apellido, c.Telefono, c.Email,
                c.Turnos.Count,
                c.Turnos
                    .Where(t => t.Fecha >= hoy && t.Estado != EstadosTurno.Cancelado)
                    .OrderBy(t => t.Fecha)
                    .Select(t => (DateTime?)t.Fecha)
                    .FirstOrDefault()))
            .ToListAsync();

        return new PagedResult<ClienteListado> { Items = items, TotalItems = total, Pagina = pagina, TamanioPagina = tamanio };
    }

    public async Task<Cliente?> ObtenerAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>Cliente con su historial de turnos (usa la relación 1 a N).</summary>
    public async Task<Cliente?> ObtenerConTurnosAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Clientes
            .AsNoTracking()
            .Include(c => c.Turnos.OrderByDescending(t => t.Fecha).ThenByDescending(t => t.Horario))
                .ThenInclude(t => t.Servicio)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> BuscarPorEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        var normalizado = email.Trim().ToLowerInvariant();
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Email == normalizado);
    }

    public async Task<ResultadoOperacion> GuardarAsync(Cliente datos)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var email = datos.Email.Trim().ToLowerInvariant();

        bool emailRepetido = await db.Clientes.AnyAsync(c => c.Email == email && c.Id != datos.Id);
        if (emailRepetido)
            return ResultadoOperacion.Falla("Ya existe otro cliente registrado con ese email.");

        Cliente cliente;
        if (datos.Id == 0)
        {
            cliente = new Cliente { FechaAlta = DateTime.Now };
            db.Clientes.Add(cliente);
        }
        else
        {
            cliente = await db.Clientes.FindAsync(datos.Id)
                      ?? throw new KeyNotFoundException($"No existe el cliente {datos.Id}.");
        }

        cliente.Nombre = datos.Nombre.Trim();
        cliente.Apellido = datos.Apellido.Trim();
        cliente.Telefono = datos.Telefono.Trim();
        cliente.Email = email;
        cliente.Notas = string.IsNullOrWhiteSpace(datos.Notas) ? null : datos.Notas.Trim();

        await db.SaveChangesAsync();
        return ResultadoOperacion.Ok(cliente.Id);
    }

    public async Task<ResultadoOperacion> EliminarAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var cliente = await db.Clientes.Include(c => c.Turnos).FirstOrDefaultAsync(c => c.Id == id);
        if (cliente is null)
            return ResultadoOperacion.Falla("El cliente no existe.");

        if (cliente.Turnos.Count > 0)
            return ResultadoOperacion.Falla($"No se puede eliminar: {cliente.NombreCompleto} tiene {cliente.Turnos.Count} turno(s) registrados. Eliminá o reasigná sus turnos primero.");

        db.Clientes.Remove(cliente);
        await db.SaveChangesAsync();
        return ResultadoOperacion.Ok();
    }
}
