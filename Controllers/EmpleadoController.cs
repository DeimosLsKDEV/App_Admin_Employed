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
    public async Task<IActionResult> Grid([FromServices] ArbolBackgroundService background)
    {
        EmpleadoModel? EmpleadoLogger = null;
        var uuidClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(uuidClaim, out Guid uuid))
        {
            EmpleadoLogger = await _EmpleadoService.ObtenerEmpleadoPorUUID(uuid);
        }

        if (!background.EstaConstruido(uuid))
        {
            return View("Cargando");
        }

        EmpleadoModel? EmpleadoConSubordinados = background.GetArbol(uuid);
        List<EmpleadoModel> ListadoEmpleado = background.ArbolALista(EmpleadoConSubordinados);

        ViewBag.ArbolEmpleado = EmpleadoConSubordinados;

        ViewBag.LoggerUserUpdatedAt = EmpleadoLogger!.UPDATEDAT == null;
        ViewBag.LoggerUserIdentificacion = EmpleadoLogger.IDENTIFICACION;
        return View(ListadoEmpleado);
    }

    [Authorize]
    [HttpGet]
    public IActionResult EstadoJerarquiaSubordinados([FromServices] ArbolBackgroundService background)
    {
        var uuidClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(uuidClaim, out Guid uuid))
            return Unauthorized();

        bool listo = background.EstaConstruido(uuid);
        return Json(new { listo });
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
        empleadoModel.GUID_SUPERVISOR = EmpleadoLogger!.UUID;
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
    public async Task<IActionResult> Add(EmpleadoModel empleado, [FromServices] ArbolBackgroundService background)
    {
        if (ModelState.IsValid)
        {

            await _EmpleadoService.AnnadirEmpleado(empleado);

            var uuidClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(uuidClaim, out Guid uuid))
            {
                await background.ReConstruirArbol(uuid);
            }

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
}
