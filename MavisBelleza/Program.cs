using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MavisBelleza.Components;
using MavisBelleza.Data;
using MavisBelleza.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Blazor Server ----------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ---------- Base de datos: EF Core + SQLite con DbContextFactory ----------
// La cadena de conexión sale de appsettings.json ("DefaultConnection").
// En Azure se pisa con la variable de entorno ConnectionStrings__DefaultConnection.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=mavisbelleza.db";
CrearCarpetaDeLaBase(connectionString);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// ---------- Servicios de la aplicación ----------
builder.Services.AddSingleton<TurnoStateService>();   // estado compartido + notificación reactiva
builder.Services.AddScoped<TurnoService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ServicioService>();
builder.Services.AddScoped<ToastService>();           // una instancia por usuario (circuito)
builder.Services.AddScoped<JsInteropService>();

var app = builder.Build();

// ---------- Aplicar migraciones pendientes al iniciar (crea la BD y carga el seed) ----------
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = dbFactory.CreateDbContext();

    if (db.Database.GetMigrations().Any())
    {
        db.Database.Migrate();          // camino normal: aplica las migraciones versionadas
    }
    else
    {
        // Red de seguridad si todavía no se generó ninguna migración (dotnet ef migrations add ...)
        app.Logger.LogWarning("No hay migraciones en el proyecto: se crea la base con EnsureCreated().");
        db.Database.EnsureCreated();
    }
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

// Si la cadena apunta a una carpeta que no existe (ej: /home/data en Azure), la crea.
static void CrearCarpetaDeLaBase(string connectionString)
{
    var dataSource = new SqliteConnectionStringBuilder(connectionString).DataSource;
    var carpeta = Path.GetDirectoryName(Path.GetFullPath(dataSource));
    if (!string.IsNullOrEmpty(carpeta))
        Directory.CreateDirectory(carpeta);
}
