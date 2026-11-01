using System.ComponentModel.DataAnnotations;

namespace MavisBelleza.Models
{
    public class TurnosModels
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string ApellidoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese una dirección de correo válida")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un servicio")]
        public string Servicio { get; set; } = string.Empty;

        public decimal Precio { get; set; }

        [Required(ErrorMessage = "Seleccione el método de pago")]
        public string MetodoPago { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione una fecha")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Seleccione un horario")]
        public string Horario { get; set; } = string.Empty;

        public string Estado { get; set; } = "Confirmado";
    }
}