using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MavisBelleza.Validation;

/// <summary>
/// Horarios de atención del studio: lunes a sábado, de 09:00 a 18:00, cada 30 minutos.
/// </summary>
public static class HorariosStudio
{
    public static readonly TimeOnly Apertura = new(9, 0);
    public static readonly TimeOnly UltimoTurno = new(18, 0);
    public const int IntervaloMinutos = 30;

    public static IReadOnlyList<string> Todos { get; } = Generar();

    private static List<string> Generar()
    {
        var lista = new List<string>();
        for (var h = Apertura; h <= UltimoTurno; h = h.AddMinutes(IntervaloMinutos))
        {
            lista.Add(h.ToString("HH:mm", CultureInfo.InvariantCulture));
            if (h == UltimoTurno) break;
        }
        return lista;
    }
}

/// <summary>
/// Valida que la fecha del turno no sea pasada, no caiga domingo
/// y no esté a más de <see cref="MaxDiasAnticipacion"/> días.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class FechaTurnoValidaAttribute : ValidationAttribute
{
    public int MaxDiasAnticipacion { get; set; } = 90;

    /// <summary>Si es true (edición), se permiten fechas pasadas para corregir turnos viejos.</summary>
    public string? PermitirPasadoSi { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not DateTime fecha)
            return ValidationResult.Success; // [Required] se encarga del vacío

        var miembro = new[] { context.MemberName ?? "Fecha" };

        if (fecha.DayOfWeek == DayOfWeek.Sunday)
            return new ValidationResult("Los domingos el studio está cerrado. Elegí de lunes a sábado.", miembro);

        bool permitirPasado = false;
        if (PermitirPasadoSi is not null)
        {
            var prop = context.ObjectType.GetProperty(PermitirPasadoSi);
            permitirPasado = prop?.GetValue(context.ObjectInstance) is true;
        }

        if (!permitirPasado && fecha.Date < DateTime.Today)
            return new ValidationResult("La fecha no puede ser anterior a hoy.", miembro);

        if (fecha.Date > DateTime.Today.AddDays(MaxDiasAnticipacion))
            return new ValidationResult($"Solo se pueden reservar turnos con hasta {MaxDiasAnticipacion} días de anticipación.", miembro);

        return ValidationResult.Success;
    }
}

/// <summary>
/// Valida que el horario tenga formato HH:mm y sea uno de los horarios de atención.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class HorarioDeAtencionAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var texto = value as string;
        if (string.IsNullOrWhiteSpace(texto))
            return ValidationResult.Success;

        var miembro = new[] { context.MemberName ?? "Horario" };

        if (!TimeOnly.TryParseExact(texto, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return new ValidationResult("El horario debe tener el formato HH:mm (ej: 14:30).", miembro);

        if (!HorariosStudio.Todos.Contains(texto))
            return new ValidationResult(
                $"Atendemos de {HorariosStudio.Todos[0]} a {HorariosStudio.Todos[^1]}, en turnos cada {HorariosStudio.IntervaloMinutos} minutos.",
                miembro);

        return ValidationResult.Success;
    }
}
