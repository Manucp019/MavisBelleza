using System.ComponentModel.DataAnnotations;
using MavisBelleza.Validation;

namespace MavisBelleza.Models;

/// <summary>
/// Modelo del formulario de turnos (ViewModel). Junta datos del cliente y del turno
/// en un solo formulario y concentra todas las reglas de validación:
///  - DataAnnotations estándar ([Required], [EmailAddress], [RegularExpression], [StringLength])
///  - Atributos personalizados ([FechaTurnoValida], [HorarioDeAtencion])
///  - Validación entre campos con IValidatableObject
///  - La disponibilidad del horario (consulta a la BD) se valida en TurnoForm con ValidationMessageStore
/// </summary>
public class TurnoFormModel : IValidatableObject
{
    public int Id { get; set; }

    /// <summary>True cuando se está editando un turno existente.</summary>
    public bool EsEdicion => Id > 0;

    // ---------- Datos del cliente ----------
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 60 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 60 caracteres")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [RegularExpression(@"^[0-9\s\-\+]{8,20}$", ErrorMessage = "Ingrese un teléfono válido (solo números, espacios o guiones)")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese una dirección de correo válida")]
    public string Email { get; set; } = string.Empty;

    // ---------- Datos del turno ----------
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un servicio")]
    public int ServicioId { get; set; }

    [Required(ErrorMessage = "Seleccione el método de pago")]
    public string MetodoPago { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione una fecha")]
    [FechaTurnoValida(MaxDiasAnticipacion = 90, PermitirPasadoSi = nameof(EsEdicion))]
    public DateTime Fecha { get; set; } = SiguienteDiaHabil(DateTime.Today);

    [Required(ErrorMessage = "Seleccione un horario")]
    [HorarioDeAtencion]
    public string Horario { get; set; } = string.Empty;

    [Required]
    public string Estado { get; set; } = EstadosTurno.Confirmado;

    [StringLength(300, ErrorMessage = "Las notas no pueden superar los 300 caracteres")]
    public string? Notas { get; set; }

    /// <summary>Validaciones que involucran más de un campo.</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Si el turno es para hoy, el horario tiene que ser posterior a la hora actual.
        if (!EsEdicion && Fecha.Date == DateTime.Today
            && TimeOnly.TryParse(Horario, out var hora)
            && hora <= TimeOnly.FromDateTime(DateTime.Now))
        {
            yield return new ValidationResult(
                "Ese horario ya pasó. Elegí un horario posterior a la hora actual.",
                new[] { nameof(Horario) });
        }

        // Un turno no puede marcarse como "Realizado" si todavía no ocurrió.
        if (Estado == EstadosTurno.Realizado && Fecha.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Un turno con fecha futura no puede estar en estado Realizado.",
                new[] { nameof(Estado) });
        }
    }

    public static DateTime SiguienteDiaHabil(DateTime desde)
        => desde.DayOfWeek == DayOfWeek.Sunday ? desde.AddDays(1) : desde;

    // ---------- Mapeos entidad <-> formulario ----------
    public static TurnoFormModel DesdeEntidad(Turno t) => new()
    {
        Id = t.Id,
        Nombre = t.Cliente?.Nombre ?? string.Empty,
        Apellido = t.Cliente?.Apellido ?? string.Empty,
        Telefono = t.Cliente?.Telefono ?? string.Empty,
        Email = t.Cliente?.Email ?? string.Empty,
        ServicioId = t.ServicioId,
        MetodoPago = t.MetodoPago,
        Fecha = t.Fecha,
        Horario = t.Horario,
        Estado = t.Estado,
        Notas = t.Notas
    };

    public static TurnoFormModel DesdeCliente(Cliente c) => new()
    {
        Nombre = c.Nombre,
        Apellido = c.Apellido,
        Telefono = c.Telefono,
        Email = c.Email
    };
}
