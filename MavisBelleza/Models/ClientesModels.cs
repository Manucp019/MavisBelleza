using System.ComponentModel.DataAnnotations;

namespace MavisBelleza.Models;

public class ClientesModels
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    public List<TurnosModels> Turnos { get; set; } = new();
}