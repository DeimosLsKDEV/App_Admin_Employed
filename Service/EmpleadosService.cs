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

        public async Task<List<EmpleadoModel>> ObtenerEmpleadosPorIdentificacion(int IDENTIFICACION)
        {
            Predicate<EmpleadoModel> predicate = new Predicate<EmpleadoModel>(
                empleado => empleado.IDENTIFICACION == IDENTIFICACION
            );

            IEnumerable<EmpleadoModel> empleados = await _repositoryEmpleado.FindAsync(
                empleado => empleado.IDENTIFICACION == IDENTIFICACION
            );

            return empleados?.ToList() ?? new List<EmpleadoModel>();
        }
        
        public async Task<EmpleadoModel> AnnadirEmpleado(EmpleadoModel EMPLEADO_NUEVO)
        {
            EmpleadoModel EMPLEADO_RESULTADO;
            EMPLEADO_RESULTADO = await _repositoryEmpleado.AddAsync(EMPLEADO_NUEVO);
            return EMPLEADO_RESULTADO;
        }
    }
}