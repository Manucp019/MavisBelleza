using MavisBelleza.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MavisBelleza.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tipos adaptados a tus clases ClientesModels y TurnosModels
    public DbSet<ClientesModels> Clientes => Set<ClientesModels>();
    public DbSet<TurnosModels> Turnos => Set<TurnosModels>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClientesModels>().HasData(
            new ClientesModels { Id = 1, Nombre = "Laura Gómez", Email = "laura@example.com" }
        );

        modelBuilder.Entity<TurnosModels>().HasData(
            new TurnosModels { Id = 1, FechaHora = DateTime.Now.AddDays(1), Servicio = "Limpieza Facial", ClienteId = 1, Confirmado = true }
        );
    }
}