using LogicaAccesoDatos;
using LogicaAccesoDatos.EF;
using LogicaAccesoDatos.Repositorios;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Railway (y otros PaaS) le indican al contenedor en qué puerto escuchar con la variable PORT.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



builder.Services.AddDbContext<ProyectoContext>(options =>
    options.UseNpgsql(ResolverCadenaConexion(builder.Configuration)));


builder.Services.AddScoped<RepositorioRifa>();
builder.Services.AddScoped<RepositorioComprador>();
builder.Services.AddScoped<RepositorioAdmin>();
builder.Services.AddHttpClient<ServicioEmail>(client => client.Timeout = TimeSpan.FromSeconds(10));


var app = builder.Build();

// Aplica las migraciones pendientes al arrancar (se activa con Database__MigrateOnStartup=true).
if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<ProyectoContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// En Railway el proxy ya redirige HTTP a HTTPS y el contenedor solo escucha HTTP: acá solo sirve en desarrollo local.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();


app.UseSession();


app.UseAuthorization();

// Healthcheck liviano (sin base de datos) para el despliegue.
app.MapGet("/healthz", () => Results.Ok("ok"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// Usa ConnectionStrings:DefaultConnection (formato Npgsql) y, si no está definida, DATABASE_URL
// (formato postgresql://usuario:clave@host:puerto/base, que es el que expone Railway).
static string? ResolverCadenaConexion(IConfiguration config)
{
    var cadena = config.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(cadena))
    {
        return cadena;
    }

    var url = config["DATABASE_URL"];
    if (string.IsNullOrWhiteSpace(url))
    {
        return null;
    }

    var uri = new Uri(url);
    var credenciales = uri.UserInfo.Split(':', 2);

    return new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.IsDefaultPort ? 5432 : uri.Port,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = Uri.UnescapeDataString(credenciales[0]),
        Password = credenciales.Length > 1 ? Uri.UnescapeDataString(credenciales[1]) : null
    }.ConnectionString;
}
