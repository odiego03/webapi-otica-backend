using System.ComponentModel.DataAnnotations;

namespace WebApi_otica.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email Obrigatorio")]
        [EmailAddress(ErrorMessage = "Formato de email invalido")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Senha é Obrigatoria")]
        [StringLength(20, ErrorMessage = "A{0} deve ter no minimo {2} e no maximo {1} caracateres")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
