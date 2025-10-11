using AppAdminEmployed.Models;
using AppAdminEmployed.Data;

namespace AppAdminEmployed.Repository
{
    public class EmpleadoRepository : GenericRepository<EmpleadoModel>
    {
        public EmpleadoRepository(AppDbContext context) : base(context)
        {
        }
    }
}