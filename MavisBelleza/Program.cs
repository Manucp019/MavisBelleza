using Microsoft.EntityFrameworkCore;
using MavisBelleza.Components;
using MavisBelleza.Data;
using MavisBelleza.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios de Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrar DbContextFactory con SQLite
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite("Data Source=mavisbelleza.db"));

// Registrar State Service para notificaciones reactivas
builder.Services.AddSingleton<TurnoStateService>();

var app = builder.Build();

// CREAR LA BASE DE DATOS SI NO EXISTE (SIN BORRAR LA EXISTENTE)
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = dbFactory.CreateDbContext();

    // Comenta o elimina EnsureDeleted():
    // db.Database.EnsureDeleted(); 

    // Mantiene la base de datos y sus turnos guardados intactos:
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();