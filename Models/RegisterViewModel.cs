using System.ComponentModel.DataAnnotations;

namespace AppAdminEmployed.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "IDENTIFICACIÓN")]
        public string IDENTIFICACION { get; set; }  = string.Empty;
    }
}