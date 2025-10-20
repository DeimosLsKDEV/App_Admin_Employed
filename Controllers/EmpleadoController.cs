using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppAdminEmployed.Models;
using AppAdminEmployed.Service;
using System.Threading.Tasks;

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

    public async Task<IActionResult> Grid()
    {
        List<EmpleadoModel> Empleado = await _EmpleadoService.ObtenerTodosLosEmpleados();
        return View(Empleado);
    }

    [HttpGet]
    public IActionResult Add()
    {
        ViewBag.MethodView = "Add";
        ViewBag.ControllerView = "Empleado";

        EmpleadoModel? empleadoModel = new EmpleadoModel();
        return PartialView(empleadoModel);
    }

    [HttpGet]
    public async Task<IActionResult> Update(
        int IDENTIFICACION
    )
    {
        ViewBag.MethodView = "Update";
        ViewBag.ControllerView = "Empleado";
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

    [HttpPost]
    public async Task<IActionResult> Update(EmpleadoModel updateEmpleado)
    {
        if (ModelState.IsValid)
        {

            await _EmpleadoService.UpdateEmpleado(updateEmpleado);

            return RedirectToAction("Grid");
        }

        return RedirectToAction("Add",updateEmpleado);
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
