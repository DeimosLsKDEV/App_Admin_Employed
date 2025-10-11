namespace AppAdminEmployed.Models;

public class EmpleadoModel
{
    public required string UUID { get; set; }
    public required int IDENTIFICACION { get; set; }

    public string? PRIMER_NOMBRE { get; set; }
    public string? SEGUNDO_NOMBRE { get; set; }
    public string? PRIMER_APELLIDO { get; set; }
    public string? SEGUNDO_APELLIDO { get; set; }
}
