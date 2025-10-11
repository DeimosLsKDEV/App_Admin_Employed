using AppAdminEmployed.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using AppAdminEmployed.Repository;
using AppAdminEmployed.Service;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

string ConnectionSQLDBString = $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};" +
                          $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                          $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
                          $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                          "TrustServerCertificate=True;";

// Add DB Conections in this case is SQL SERVER
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(ConnectionSQLDBString));

builder.Services.AddScoped<EmpleadosService>();
builder.Services.AddScoped<EmpleadoRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Generated automatic DB Requeriments into AppDbContext
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
