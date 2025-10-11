using Microsoft.EntityFrameworkCore;
using AppAdminEmployed.Models;

namespace MiAppMvc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<EmpleadoModel> DbEmpleado { get; set; }
    }
}