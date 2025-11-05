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
        [Display(Name = "IDENTIFICACION")]
        public string IDENTIFICACION { get; set; } = string.Empty;
        [Display(Name = "PRIMER NOMBRE")]
        public string? PRIMER_NOMBRE { get; set; }
        [Display(Name = "SEGUNDO NOMBRE")]
        public string? SEGUNDO_NOMBRE { get; set; }
        [Display(Name = "PRIMER APELLIDO")]
        public string? PRIMER_APELLIDO { get; set; }
        [Display(Name = "SEGUNDO APELLIDO")]
        public string? SEGUNDO_APELLIDO { get; set; }
        public Guid? GUID_SUPERVISOR { get; set; }

        [ForeignKey("GUID_SUPERVISOR")]
        public EmpleadoModel? Supervisor { get; set; }
        public ICollection<EmpleadoModel>? Subordinados { get; set; }

        [NotMapped]
        [Display(Name = "NOMBRE COMPLETO")]
        public string NOMBRE_COMPLETO
        {
            get
            {
                return $"{PRIMER_NOMBRE} {SEGUNDO_NOMBRE} {PRIMER_APELLIDO} {SEGUNDO_APELLIDO}".Replace("  ", " ").Trim();
            }
        }

        [NotMapped]
        public bool IS_EDITABLE { get; set; } = false;
        [NotMapped]
        public bool IS_DELETABLE { get; set; } = false;
        public DateTime CREATEDAT { get; set; } = DateTime.UtcNow;
        public DateTime? UPDATEDAT { get; set; }
        [NotMapped]
        public Guid? SUPERVISOR_UUID_VISTA { get; set; }

        [NotMapped]
        public List<EmpleadoModel>? Breadcrumb { get; set; } = new();

        [NotMapped]
        public IFormFile FotoEmpleado { get; set; }
        public byte[]? ImagenEmpleado { get; set; }
    }
}