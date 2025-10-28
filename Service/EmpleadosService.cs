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

        public async Task<EmpleadoModel> ObtenerEmpleadosPorIdentificacion(string IDENTIFICACION)
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
            EMPLEADO_NUEVO.CREATEDAT = DateTime.UtcNow;
            EMPLEADO_RESULTADO = await _repositoryEmpleado.AddAsync(EMPLEADO_NUEVO);
            return EMPLEADO_RESULTADO;
        }

        public async Task<EmpleadoModel> UpdateEmpleado(EmpleadoModel EMPLEADO_ACTUALIZAR)
        {
            try
            {
                EMPLEADO_ACTUALIZAR.UPDATEDAT = DateTime.UtcNow;
                await _repositoryEmpleado.UpdateAsync(EMPLEADO_ACTUALIZAR);
                return EMPLEADO_ACTUALIZAR;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task DeleteEmpleado(EmpleadoModel EMPLEADO_ELIMINAR)
        {
            try
            {
                await _repositoryEmpleado.DeleteAsync(EMPLEADO_ELIMINAR);
            }
            catch (Exception ex)
            {
            }

        }

        public async Task<EmpleadoModel?> ConstruirJerarquiaSubordinados(Guid supervisorUuid)
        {
            List<EmpleadoModel> ListaEmpleados = (await _repositoryEmpleado.GetAllAsync()).ToList();
            var lookup = ListaEmpleados.ToDictionary(e => e.UUID);

            foreach (EmpleadoModel emp in ListaEmpleados)
            {
                emp.Subordinados = new List<EmpleadoModel>();
            }

            foreach (var emp in ListaEmpleados)
            {
                if (emp.GUID_SUPERVISOR != null && lookup.ContainsKey(emp.GUID_SUPERVISOR.Value))
                {
                    EmpleadoModel? jefe = lookup[emp.GUID_SUPERVISOR.Value];
                    jefe.Subordinados!.Add(emp);
                    emp.Supervisor = jefe;
                }
            }

            if (lookup.ContainsKey(supervisorUuid))
            {
                var root = lookup[supervisorUuid];
                if (root.Subordinados != null)
                {
                    foreach (var sub in root.Subordinados)
                    {
                        MarcarEditable(sub, supervisorUuid, true);
                    }
                }
                return root;
            }

            return null;
        }


        private void MarcarEditable(EmpleadoModel nodo, Guid supervisorUuid, bool esDirecto = true)
        {
            if (esDirecto)
            {
                nodo.IS_EDITABLE = true;
                nodo.IS_DELETABLE = true;
            }
            else
            {
                nodo.IS_EDITABLE = false;
                nodo.IS_DELETABLE = true;
            }

            if (nodo.Subordinados != null)
            {
                foreach (var sub in nodo.Subordinados)
                {
                    MarcarEditable(sub, supervisorUuid, false);
                }
            }
        }
    }
}