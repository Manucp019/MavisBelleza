using System.ComponentModel.DataAnnotations;

namespace MavisBelleza.Models;

public class TurnosModels
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha es requerida")]
    public DateTime FechaHora { get; set; } = DateTime.Now.AddDays(1);

    [Required(ErrorMessage = "Seleccione un servicio")]
    public string Servicio { get; set; } = string.Empty;

    public bool Confirmado { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente")]
    public int ClienteId { get; set; }

    // Cambiado de Cliente? a ClientesModels? para coincidir con la clase
    public ClientesModels? Cliente { get; set; }
}