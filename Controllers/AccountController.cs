using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using AppAdminEmployed.Models;
using System.Security.Claims;
using AppAdminEmployed.Service;

namespace AppAdminEmployed.Controllers;
public class AccountController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly EmpleadosService _EmpleadoService;

    public AccountController(
        ILogger<HomeController> logger,
        EmpleadosService EmpleadoService
    )
    {
        _logger = logger;
        _EmpleadoService = EmpleadoService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);


        EmpleadoModel EmpleadoEncontrado = await _EmpleadoService.ObtenerEmpleadosPorIdentificacion(model.IDENTIFICACION);

        if (EmpleadoEncontrado != null)
        {
            // 🔹 Crear claims (aquí puedes guardar el nombre completo o UUID)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, EmpleadoEncontrado.UUID.ToString()),
                new Claim(ClaimTypes.Name, $"{EmpleadoEncontrado.PRIMER_NOMBRE} {EmpleadoEncontrado.PRIMER_APELLIDO}"),
                new Claim("Identificacion", EmpleadoEncontrado.IDENTIFICACION.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Grid", "Empleado");
        }

        ModelState.AddModelError("", "Identificación no encontrada");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

     [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var empleado = new EmpleadoModel
        {
            IDENTIFICACION = model.IDENTIFICACION
        };
        
        await _EmpleadoService.AnnadirEmpleado(empleado);

        return RedirectToAction("Login");
    }
}