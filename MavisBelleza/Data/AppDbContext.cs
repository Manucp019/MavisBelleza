using Microsoft.EntityFrameworkCore;
using MavisBelleza.Models;

namespace MavisBelleza.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TurnosModels> Turnos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Sin datos iniciales (Seed Data). La base de datos iniciará vacía.
    }
}