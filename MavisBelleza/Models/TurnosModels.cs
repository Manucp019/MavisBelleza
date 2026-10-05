namespace MavisBelleza.Models
{
    public class TurnosModels
    {
        public int Id { get; set; }

        // Campos de Cliente
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidoCliente { get; set; } = string.Empty;

        // Selección de Servicio y Precio
        public string Servicio { get; set; } = string.Empty;
        public decimal Precio { get; set; }

        // Campos de Turno
        public DateTime Fecha { get; set; } = DateTime.Today;
        public TimeSpan Hora { get; set; } = new TimeSpan(9, 0, 0); // 09:00 por defecto
        public string Estado { get; set; } = "Confirmado";
    }
}