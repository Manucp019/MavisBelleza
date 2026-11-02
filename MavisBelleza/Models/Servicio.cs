using System.ComponentModel.DataAnnotations;

namespace MavisBelleza.Models;

/// <summary>
/// Tratamiento que ofrece el studio (Lifting, Laminado, Perfilado...).
/// Un servicio puede estar en muchos turnos (relación 1 a N).
/// </summary>
public class Servicio
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(400)]
    public string? Descripcion { get; set; }

    [Range(10, 300)]
    public int DuracionMinutos { get; set; }

    [Range(0, 10_000_000)]
    public decimal Precio { get; set; }

    public bool Activo { get; set; } = true;

    public List<Turno> Turnos { get; set; } = new();

    public string DuracionTexto => DuracionMinutos >= 60
        ? $"{DuracionMinutos / 60}h {(DuracionMinutos % 60 > 0 ? $"{DuracionMinutos % 60}m" : "")}".Trim()
        : $"{DuracionMinutos} min";
}
