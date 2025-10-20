using System.Linq.Expressions;
using AppAdminEmployed.Models;
using AppAdminEmployed.Repository;

namespace AppAdminEmployed.Service
{
    public class EmpleadosService
    {
        private readonly EmpleadoRepository _repositoryEmpleado;

        public EmpleadosService(EmpleadoRepository repositoryEmpleado)
        {
            _repositoryEmpleado = repositoryEmpleado;
        }

        public async Task<List<EmpleadoModel>> ObtenerTodosLosEmpleados()
        { 
            IEnumerable<EmpleadoModel> todos_empleados = await _repositoryEmpleado.GetAllAsync();
            return todos_empleados?.ToList() ?? new List<EmpleadoModel>();
        }

        public async Task<EmpleadoModel> ObtenerEmpleadosPorIdentificacion(int IDENTIFICACION)
        {
            Expression<Func<EmpleadoModel, bool>> filtro = 
                empleado => empleado.IDENTIFICACION == IDENTIFICACION;

            IEnumerable<EmpleadoModel> empleados = await _repositoryEmpleado.FindAsync(filtro);

            return empleados.First();
        }

        public async Task<EmpleadoModel> ObtenerEmpleadoPorUUID(Guid UUID)
        {
            Expression<Func<EmpleadoModel, bool>> filtro =
                empleado => empleado.UUID == UUID;
                
            IEnumerable<EmpleadoModel> empleados = await _repositoryEmpleado.FindAsync(filtro);

            return empleados.First();
        }

        public async Task<EmpleadoModel> AnnadirEmpleado(EmpleadoModel EMPLEADO_NUEVO)
        {
            EmpleadoModel EMPLEADO_RESULTADO;
            EMPLEADO_NUEVO.UUID = Guid.NewGuid();
            EMPLEADO_RESULTADO = await _repositoryEmpleado.AddAsync(EMPLEADO_NUEVO);
            return EMPLEADO_RESULTADO;
        }
        
        public async Task<EmpleadoModel> UpdateEmpleado(EmpleadoModel EMPLEADO_ACTUALIZAR)
        {
            try
            {
                await _repositoryEmpleado.UpdateAsync(EMPLEADO_ACTUALIZAR);
                return EMPLEADO_ACTUALIZAR;
            }
            catch (Exception ex)
            {

                return null;
            }
            
        }
    }
}