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
    public async Task<IActionResult> Add()
    {
        EmpleadoModel? empleadoModel = null;
        return PartialView(empleadoModel);
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
