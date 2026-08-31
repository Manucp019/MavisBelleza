using Microsoft.EntityFrameworkCore; // <-- Agrega esta línea arriba de todo
using MavisBelleza.Components;
using MavisBelleza.Data;
using MavisBelleza.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// UNIDAD 3: Registrar DbContextFactory con SQLite
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite("Data Source=mavisbelleza.db"));

// UNIDAD 1: Registrar State Service para notificación reactiva
builder.Services.AddSingleton<TurnoStateService>();

var app = builder.Build();

// Crear la base de datos automáticamente si no existe al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = dbFactory.CreateDbContext();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
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