using System.Linq.Expressions;
using AppAdminEmployed.Models;
using AppAdminEmployed.Repository;
using AppAdminEmployed.Services;
using Microsoft.Extensions.Caching.Memory;

namespace AppAdminEmployed.Service
{
    public class EmpleadosService
    {
        private readonly EmpleadoRepository _repositoryEmpleado;
        private readonly IMemoryCache _cache;
        private readonly CSVService _csvService;

        public EmpleadosService(
            EmpleadoRepository repositoryEmpleado,
            IMemoryCache cache,
            CSVService csvService
        ){
            _repositoryEmpleado = repositoryEmpleado;
            _cache = cache;
            _csvService = csvService;
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


        public async Task<List<EmpleadoModel>> CargarDesdeCsvAsync(IFormFile archivo)
        {
            var filas = await _csvService.LeerCsvAsync(archivo.OpenReadStream());
            var empleados = new List<EmpleadoModel>();

            foreach (var fila in filas)
            {
                try
                {
                    var empleado = new EmpleadoModel
                    {
                        UUID = Guid.NewGuid(),
                        IDENTIFICACION = fila.GetValueOrDefault("IDENTIFICACION") ?? string.Empty,
                        PRIMER_NOMBRE = fila.GetValueOrDefault("PRIMER_NOMBRE"),
                        SEGUNDO_NOMBRE = fila.GetValueOrDefault("SEGUNDO_NOMBRE"),
                        PRIMER_APELLIDO = fila.GetValueOrDefault("PRIMER_APELLIDO"),
                        SEGUNDO_APELLIDO = fila.GetValueOrDefault("SEGUNDO_APELLIDO"),
                        GUID_SUPERVISOR = Guid.TryParse(fila.GetValueOrDefault("GUID_SUPERVISOR"), out var guidSup)
                            ? guidSup
                            : (Guid?)null,
                        CREATEDAT = DateTime.UtcNow
                    };

                    empleados.Add(empleado);
                }
                catch
                {
                    // Ignorar fila malformada
                    continue;
                }
            }

            _cache.Set("EmpleadosCache", empleados, TimeSpan.FromMinutes(15));

            return empleados;
        }
        
        public List<EmpleadoModel>? ObtenerDesdeCache()
        {
            _cache.TryGetValue("empleados_csv", out List<EmpleadoModel>? empleados);
            return empleados;
        }
        
    }
}