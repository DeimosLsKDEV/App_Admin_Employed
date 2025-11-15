using AppAdminEmployed.Models;
using AppAdminEmployed.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
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
            ViewBag.ActionResult = Url.Action("Grid", "Empleado");
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
        EmpleadoModel? JefeEmpleado;
        try
        {
            UpdatedEmpleado = await _EmpleadoService.ObtenerEmpleadosPorIdentificacion(IDENTIFICACION);
            if (UpdatedEmpleado.GUID_SUPERVISOR is Guid guid)
            {
                JefeEmpleado = await _EmpleadoService.ObtenerEmpleadoPorUUID(UpdatedEmpleado.GUID_SUPERVISOR.Value);
                UpdatedEmpleado.Supervisor = JefeEmpleado;
            }

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

    [HttpGet]
    public async Task<IActionResult> Jerarquia(Guid? uuidBuscado, [FromServices] ArbolBackgroundService background)
    {
        EmpleadoModel? empleadoBuscado = null;

        var uuidClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (uuidBuscado == null && Guid.TryParse(uuidClaim, out Guid uuid))
            uuidBuscado = uuid;

        Guid UUIDUsuarioBuscado = uuidBuscado ?? Guid.Empty;

        if (!background.EstaConstruido(UUIDUsuarioBuscado))
        {
            var empleadoTemp = await _EmpleadoService.ObtenerEmpleadoPorUUID(UUIDUsuarioBuscado);
            ViewBag.ActionResult = Url.Action("Jerarquia", "Empleado", new { uuidBuscado });
            await background.EncolarConstruccion(empleadoTemp!.UUID);
            return View("Cargando");
        }

        EmpleadoModel? arbol = background.GetArbol(UUIDUsuarioBuscado);
        List<EmpleadoModel> listado = background.ArbolALista(arbol);
        empleadoBuscado = listado.FirstOrDefault(e => e.UUID == UUIDUsuarioBuscado);
        if (empleadoBuscado == null)
            return NotFound("Empleado no encontrado en el árbol.");
        empleadoBuscado.Subordinados = listado
            .Where(e => e.GUID_SUPERVISOR == empleadoBuscado.UUID)
            .ToList();

        empleadoBuscado.SUPERVISOR_UUID_VISTA = empleadoBuscado.GUID_SUPERVISOR;
        empleadoBuscado.Breadcrumb = ConstruirBreadcrumb(listado, empleadoBuscado);

        return View(empleadoBuscado);
    }

    private List<EmpleadoModel> ConstruirBreadcrumb(List<EmpleadoModel> lista, EmpleadoModel actual)
    {
        var camino = new List<EmpleadoModel>();
        var nodo = actual;

        while (nodo != null)
        {
            camino.Add(new EmpleadoModel
            {
                UUID = nodo.UUID,
                PRIMER_NOMBRE = nodo.PRIMER_NOMBRE,
                SEGUNDO_NOMBRE = nodo.SEGUNDO_NOMBRE,
                PRIMER_APELLIDO = nodo.PRIMER_APELLIDO,
                SEGUNDO_APELLIDO = nodo.SEGUNDO_APELLIDO
            });

            nodo = lista.FirstOrDefault(e => e.UUID == nodo.GUID_SUPERVISOR);
        }

        camino.Reverse();
        return camino;
    }

    [HttpGet]
    public async Task<IActionResult> Buscador(Guid? uuidBuscado, [FromServices] ArbolBackgroundService background)
    {
        return PartialView();
    }


    [HttpPost]
    public async Task<IActionResult> Save(EmpleadoModel model)
    {
        try
        {
            if (model.FotoEmpleado == null || model.FotoEmpleado.Length == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "No se ha subido ninguna imagen."
                });
            }

            using (var ms = new MemoryStream())
            {
                await model.FotoEmpleado.CopyToAsync(ms);
                ms.Position = 0;

                using (var image = await Image.LoadAsync(ms))
                {
                    // Redimensionar si es muy grande
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(600, 600)
                    }));

                    using (var outStream = new MemoryStream())
                    {
                        // Guardar con compresión JPEG (calidad 50)
                        var encoder = new JpegEncoder { Quality = 50 };
                        await image.SaveAsJpegAsync(outStream, encoder);

                        HttpContext.Session.Set("ImagenEmpleado", outStream.ToArray());
                    }
                }
            }

            return Json(new
            {
                success = true,
                message = "Imagen cargada y almacenada en sesión correctamente."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = "Ocurrió un error al procesar la imagen.",
                error = ex.Message
            });
        }
    }

    [HttpGet]
    public IActionResult Imagen()
    {
        var imagenBytes = HttpContext.Session.Get("ImagenEmpleado");
        if (imagenBytes != null)
            return File(imagenBytes, "image/jpeg");
    
        return NotFound();
    }
}
