using Microsoft.EntityFrameworkCore;
using MavisBelleza.Models;

namespace MavisBelleza.Data;

/// <summary>
/// Datos iniciales (seed) que se insertan con la migración.
/// Los valores tienen que ser fijos (sin DateTime.Now) para que EF no genere
/// cambios nuevos en cada migración.
/// </summary>
public static class SeedData
{
    public static void Aplicar(ModelBuilder mb)
    {
        mb.Entity<Servicio>().HasData(
            new Servicio
            {
                Id = 1,
                Nombre = "Lifting de Pestañas",
                DuracionMinutos = 90,
                Precio = 30000,
                Activo = true,
                Descripcion = "Arquea y da curvatura natural a las pestañas desde la raíz. Incluye nutrición con botox/keratina y tinte."
            },
            new Servicio
            {
                Id = 2,
                Nombre = "Laminado de Cejas",
                DuracionMinutos = 60,
                Precio = 30000,
                Activo = true,
                Descripcion = "Diseño personalizado según la morfología del rostro (visagismo), con pinza o cera suave."
            },
            new Servicio
            {
                Id = 4,
                Nombre = "Promo Lifting + Perfilado",
                DuracionMinutos = 90,
                Precio = 40000,
                Activo = true,
                Descripcion = "Lifting de pestañas con nutrición y tinte, más perfilado de cejas personalizado."
            },
            new Servicio
            {
                Id = 5,
                Nombre = "Promo Laminado + Lifting",
                DuracionMinutos = 90,
                Precio = 55000,
                Activo = true,
                Descripcion = "Laminado de cejas con hidratación, más lifting de pestañas con nutrición y tinte."
            });

        var alta = new DateTime(2026, 10, 1, 10, 0, 0);

        mb.Entity<Cliente>().HasData(
            new Cliente { Id = 1, Nombre = "Ana", Apellido = "Gómez", Telefono = "11 2345 6789", Email = "ana.gomez@example.com", FechaAlta = alta },
            new Cliente { Id = 2, Nombre = "Lucía", Apellido = "Fernández", Telefono = "11 3456 7890", Email = "lucia.fernandez@example.com", FechaAlta = alta },
            new Cliente { Id = 3, Nombre = "Sofía", Apellido = "Martínez", Telefono = "11 4567 8901", Email = "sofia.martinez@example.com", FechaAlta = alta },
            new Cliente { Id = 4, Nombre = "Valentina", Apellido = "López", Telefono = "11 5678 9012", Email = "valentina.lopez@example.com", FechaAlta = alta },
            new Cliente { Id = 5, Nombre = "Camila", Apellido = "Rodríguez", Telefono = "11 6789 0123", Email = "camila.rodriguez@example.com", FechaAlta = alta });

        var creado = new DateTime(2026, 10, 20, 12, 0, 0);

        mb.Entity<Turno>().HasData(
            new Turno { Id = 1, ClienteId = 1, ServicioId = 1, Fecha = new DateTime(2026, 10, 24), Horario = "10:00", Precio = 30000, MetodoPago = "Efectivo", Estado = EstadosTurno.Realizado, FechaCreacion = creado },
            new Turno { Id = 2, ClienteId = 2, ServicioId = 3, Fecha = new DateTime(2026, 10, 28), Horario = "11:30", Precio = 15000, MetodoPago = "Transferencia", Estado = EstadosTurno.Realizado, FechaCreacion = creado },
            new Turno { Id = 3, ClienteId = 3, ServicioId = 2, Fecha = new DateTime(2026, 11, 5), Horario = "15:00", Precio = 30000, MetodoPago = "Mercado Pago", Estado = EstadosTurno.Confirmado, FechaCreacion = creado },
            new Turno { Id = 4, ClienteId = 1, ServicioId = 3, Fecha = new DateTime(2026, 11, 6), Horario = "09:30", Precio = 15000, MetodoPago = "Efectivo", Estado = EstadosTurno.Pendiente, FechaCreacion = creado },
            new Turno { Id = 5, ClienteId = 4, ServicioId = 1, Fecha = new DateTime(2026, 11, 7), Horario = "16:30", Precio = 30000, MetodoPago = "Transferencia", Estado = EstadosTurno.Confirmado, FechaCreacion = creado },
            new Turno { Id = 6, ClienteId = 5, ServicioId = 2, Fecha = new DateTime(2026, 11, 10), Horario = "12:00", Precio = 30000, MetodoPago = "Efectivo", Estado = EstadosTurno.Cancelado, FechaCreacion = creado },
            new Turno { Id = 7, ClienteId = 2, ServicioId = 1, Fecha = new DateTime(2026, 11, 12), Horario = "10:30", Precio = 30000, MetodoPago = "Mercado Pago", Estado = EstadosTurno.Confirmado, FechaCreacion = creado },
            new Turno { Id = 8, ClienteId = 3, ServicioId = 3, Fecha = new DateTime(2026, 11, 14), Horario = "17:00", Precio = 15000, MetodoPago = "Transferencia", Estado = EstadosTurno.Pendiente, FechaCreacion = creado });
    }
}
