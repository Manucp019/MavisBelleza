namespace MavisBelleza.Models;

/// <summary>
/// Turno reservado. Pertenece a un Cliente y a un Servicio (dos claves foráneas).
/// La validación de formulario se hace sobre <see cref="TurnoFormModel"/>.
/// </summary>
public class Turno
{
    public int Id { get; set; }

    // FK -> Cliente
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    // FK -> Servicio
    public int ServicioId { get; set; }
    public Servicio? Servicio { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Today;

    /// <summary>Hora del turno en formato "HH:mm".</summary>
    public string Horario { get; set; } = string.Empty;

    /// <summary>Precio congelado al momento de reservar (si cambia el precio del servicio, el turno no cambia).</summary>
    public decimal Precio { get; set; }

    public string MetodoPago { get; set; } = string.Empty;

    public string Estado { get; set; } = EstadosTurno.Confirmado;

    public string? Notas { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}

public static class EstadosTurno
{
    public const string Pendiente = "Pendiente";
    public const string Confirmado = "Confirmado";
    public const string Realizado = "Realizado";
    public const string Cancelado = "Cancelado";

    public static readonly string[] Todos = { Pendiente, Confirmado, Realizado, Cancelado };
}

public static class MetodosPago
{
    public static readonly string[] Todos = { "Efectivo", "Transferencia", "Mercado Pago" };
}

/// <summary>Datos que manda TurnoCard al padre cuando se pide cambiar el estado de un turno.</summary>
public record CambioEstadoTurno(Turno Turno, string NuevoEstado);
