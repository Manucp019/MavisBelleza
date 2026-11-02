using Microsoft.EntityFrameworkCore;
using MavisBelleza.Data;
using MavisBelleza.Models;

namespace MavisBelleza.Services;

/// <summary>Catálogo de servicios (tratamientos) del studio.</summary>
public class ServicioService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public ServicioService(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<List<Servicio>> ListarActivosAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Servicios.AsNoTracking()
            .Where(s => s.Activo)
            .OrderBy(s => s.Id)
            .ToListAsync();
    }
}
