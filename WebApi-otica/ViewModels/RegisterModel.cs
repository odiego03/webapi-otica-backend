using System.ComponentModel.DataAnnotations;

namespace WebApi_otica.ViewModels
{
    public class RegisterModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [Compare("Password", ErrorMessage = "Senhas nao coferem")]
        public string ConfirmPassword { get; set; }
    }
}
