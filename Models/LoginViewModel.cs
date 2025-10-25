using System.ComponentModel.DataAnnotations;

namespace AppAdminEmployed.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "IDENTIFICACIÓN")]
        public string IDENTIFICACION { get; set; } = string.Empty;
    }
}