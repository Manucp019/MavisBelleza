using Microsoft.EntityFrameworkCore;
using MavisBelleza.Data;
using MavisBelleza.Models;

namespace MavisBelleza.Services;

/// <summary>
/// State Service con notificación reactiva (patrón observer).
/// Es Singleton: guarda un resumen compartido por todos los usuarios conectados
/// (turnos de hoy y pendientes). Cuando alguien crea, edita o borra un turno,
/// se recalcula y se dispara <see cref="OnChange"/>; los componentes suscriptos
/// (NavMenu, Home, ListadoTurnos) se vuelven a renderizar solos.
/// </summary>
public class TurnoStateService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private bool _inicializado;

    public TurnoStateService(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;

    /// <summary>Se dispara cada vez que cambian los turnos.</summary>
    public event Action? OnChange;

    public int TurnosHoy { get; private set; }
    public int TurnosPendientes { get; private set; }
    public int TurnosProximos { get; private set; }
    public DateTime UltimaActualizacion { get; private set; }

    /// <summary>Carga el resumen la primera vez que algún componente lo necesita.</summary>
    public async Task AsegurarCargadoAsync()
    {
        if (_inicializado) return;
        await RecalcularAsync();
    }

    /// <summary>Llamar después de cualquier alta, baja o modificación de turnos.</summary>
    public async Task NotificarCambiosAsync()
    {
        await RecalcularAsync();
        OnChange?.Invoke();
    }

    private async Task RecalcularAsync()
    {
        await _lock.WaitAsync();
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var hoy = DateTime.Today;
            var activos = db.Turnos.Where(t => t.Estado != EstadosTurno.Cancelado);

            TurnosHoy = await activos.CountAsync(t => t.Fecha == hoy);
            TurnosPendientes = await activos.CountAsync(t => t.Estado == EstadosTurno.Pendiente);
            TurnosProximos = await activos.CountAsync(t => t.Fecha >= hoy && t.Estado != EstadosTurno.Realizado);
            UltimaActualizacion = DateTime.Now;
            _inicializado = true;
        }
        finally
        {
            _lock.Release();
        }
    }
}
