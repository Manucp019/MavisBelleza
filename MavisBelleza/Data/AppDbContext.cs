using Microsoft.EntityFrameworkCore;
using MavisBelleza.Models;

namespace MavisBelleza.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Turno> Turnos => Set<Turno>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Cliente ----------
        modelBuilder.Entity<Cliente>(e =>
        {
            e.Property(c => c.Nombre).HasMaxLength(60).IsRequired();
            e.Property(c => c.Apellido).HasMaxLength(60).IsRequired();
            e.Property(c => c.Telefono).HasMaxLength(20).IsRequired();
            e.Property(c => c.Email).HasMaxLength(120).IsRequired();
            e.Property(c => c.Notas).HasMaxLength(300);
            e.HasIndex(c => c.Email).IsUnique();     // no se repiten clientes con el mismo email
            e.Ignore(c => c.NombreCompleto);
        });

        // ---------- Servicio ----------
        modelBuilder.Entity<Servicio>(e =>
        {
            e.Property(s => s.Nombre).HasMaxLength(80).IsRequired();
            e.Property(s => s.Descripcion).HasMaxLength(400);
            // SQLite no tiene tipo decimal nativo: se guarda como REAL para poder ordenar y sumar
            e.Property(s => s.Precio).HasConversion<double>();
            e.Ignore(s => s.DuracionTexto);
        });

        // ---------- Turno (relaciones) ----------
        modelBuilder.Entity<Turno>(e =>
        {
            e.Property(t => t.Horario).HasMaxLength(5).IsRequired();
            e.Property(t => t.MetodoPago).HasMaxLength(30).IsRequired();
            e.Property(t => t.Estado).HasMaxLength(20).IsRequired();
            e.Property(t => t.Notas).HasMaxLength(300);
            e.Property(t => t.Precio).HasConversion<double>();

            // Cliente 1 ---- N Turnos
            e.HasOne(t => t.Cliente)
             .WithMany(c => c.Turnos)
             .HasForeignKey(t => t.ClienteId)
             .OnDelete(DeleteBehavior.Restrict);   // no se puede borrar un cliente con turnos

            // Servicio 1 ---- N Turnos
            e.HasOne(t => t.Servicio)
             .WithMany(s => s.Turnos)
             .HasForeignKey(t => t.ServicioId)
             .OnDelete(DeleteBehavior.Restrict);

            // Índice para búsquedas por fecha y para evitar dos turnos activos en el mismo horario
            e.HasIndex(t => new { t.Fecha, t.Horario });
        });

        SeedData.Aplicar(modelBuilder);
    }
}
