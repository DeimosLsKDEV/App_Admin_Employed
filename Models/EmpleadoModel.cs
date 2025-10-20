using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAdminEmployed.Models
{
    [Table("Empleados")]
    public class EmpleadoModel
    {
        [Key]
        public Guid UUID { get; set; }
        [Required]
        public int? IDENTIFICACION { get; set; } = null;

        public string? PRIMER_NOMBRE { get; set; }
        public string? SEGUNDO_NOMBRE { get; set; }
        public string? PRIMER_APELLIDO { get; set; }
        public string? SEGUNDO_APELLIDO { get; set; }
    }
}