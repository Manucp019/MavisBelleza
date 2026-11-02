using System.ComponentModel.DataAnnotations;

namespace MavisBelleza.Models;

/// <summary>
/// Cliente del studio. Un cliente puede tener muchos turnos (relación 1 a N).
/// </summary>
public class Cliente
{
    public int Id { get; set; }

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
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "Las notas no pueden superar los 300 caracteres")]
    public string? Notas { get; set; }

    public DateTime FechaAlta { get; set; } = DateTime.Now;

    // Propiedad de navegación: turnos de este cliente
    public List<Turno> Turnos { get; set; } = new();

    public string NombreCompleto => $"{Nombre} {Apellido}";
}
