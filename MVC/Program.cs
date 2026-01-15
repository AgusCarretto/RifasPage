using LogicaAccesoDatos.EF; // Asegúrate de tener estas referencias
using LogicaAccesoDatos.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ProyectoContext>(options =>
{
    // Usamos la misma cadena que pusiste en tu clase Context
    options.UseSqlServer("SERVER=localhost;DATABASE=RifasLondres;Trusted_Connection=True;TrustServerCertificate=True;");
});

builder.Services.AddScoped<RepositorioRifa>();
builder.Services.AddScoped<RepositorioComprador>();
builder.Services.AddScoped<RepositorioAdmin>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
