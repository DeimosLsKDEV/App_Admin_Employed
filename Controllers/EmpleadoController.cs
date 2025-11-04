using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppAdminEmployed.Models;
using AppAdminEmployed.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AppAdminEmployed.Controllers;

public class EmpleadoController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly EmpleadosService _EmpleadoService;

    public EmpleadoController(
        ILogger<HomeController> logger,
        EmpleadosService EmpleadoService
    )
    {
        _logger = logger;
        _EmpleadoService = EmpleadoService;
    }

    [Authorize]
    public async Task<IActionResult> Grid()
    {
        EmpleadoModel EmpleadoLogger = null;
        var uuidClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(uuidClaim, out Guid uuid))
        {
            EmpleadoLogger = await _EmpleadoService.ObtenerEmpleadoPorUUID(uuid);
        }

        ViewBag.LoggerUserUpdatedAt = EmpleadoLogger!.UPDATEDAT == null;
        ViewBag.LoggerUserIdentificacion = EmpleadoLogger.IDENTIFICACION;
        List<EmpleadoModel> Empleado = await _EmpleadoService.ObtenerTodosLosEmpleados();
        return View(Empleado);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        EmpleadoModel? EmpleadoLogger = null;
        ViewBag.MethodView = "Add";
        ViewBag.ControllerView = "Empleado";
        ViewBag.TitleView = "Añadir subordinado.";

        var uuidClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(uuidClaim, out Guid uuid))
        {
            EmpleadoLogger = await _EmpleadoService.ObtenerEmpleadoPorUUID(uuid);
        }

        EmpleadoModel? empleadoModel = new EmpleadoModel();
        empleadoModel.GUID_SUPERVISOR = EmpleadoLogger!.GUID_SUPERVISOR;
        empleadoModel.Supervisor = EmpleadoLogger;
        return PartialView(empleadoModel);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Update(
        string IDENTIFICACION
    )
    {
        ViewBag.MethodView = "Update";
        ViewBag.ControllerView = "Empleado";
        ViewBag.TitleView = "Actualizar empleado.";

        EmpleadoModel? UpdatedEmpleado;
        try
        {
            UpdatedEmpleado = await _EmpleadoService.ObtenerEmpleadosPorIdentificacion(IDENTIFICACION);

            if (UpdatedEmpleado == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Empleado no encontrado",
                    errorCode = "NOT_FOUND"
                });
            }
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                errorCode = "EXCEPTION",
                mensaje = "Hubo un eror inesperado: " + ex.Message
            });
        }
        return PartialView("Add", UpdatedEmpleado);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add(EmpleadoModel empleado)
    {
        if (ModelState.IsValid)
        {

            await _EmpleadoService.AnnadirEmpleado(empleado);

            return RedirectToAction("Grid");
        }

        return View(empleado);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Update(EmpleadoModel updateEmpleado)
    {
        if (ModelState.IsValid)
        {

            await _EmpleadoService.UpdateEmpleado(updateEmpleado);

            return RedirectToAction("Grid");
        }

        return RedirectToAction("Add", updateEmpleado);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(string uuidEmpleado)
    {
        EmpleadoModel? empleadoDelete;
        Guid uuid = Guid.Parse(uuidEmpleado);
 
        empleadoDelete = await _EmpleadoService.ObtenerEmpleadoPorUUID(uuid);
        if (empleadoDelete == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Empleado no encontrado",
                errorCode = "NOT_FOUND"
            });
        }
        await _EmpleadoService.DeleteEmpleado(empleadoDelete);
        return RedirectToAction("Grid");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No se subió ningún archivo.");
        var empleados = await _EmpleadoService.CargarDesdeCsvAsync(file);
        return Ok(new
        {
            mensaje = $"Se cargaron {empleados.Count} empleados desde CSV y se guardaron en caché.",
            total = empleados.Count
        });
    }
    [HttpGet]
    public IActionResult GetCachedEmpleados()
    {
        var empleados = _EmpleadoService.ObtenerDesdeCache();
        if (empleados == null)
            return NotFound("No hay datos de empleados en caché.");
        return Json(empleados);
    }
}
