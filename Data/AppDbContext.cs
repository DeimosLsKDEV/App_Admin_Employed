using Microsoft.EntityFrameworkCore;
using AppAdminEmployed.Models;

namespace AppAdminEmployed.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<EmpleadoModel> Empleados { get; set; }
    }
}