using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class LoginModel
    {
        [Display(Name ="E-mail")]
        [Required(ErrorMessage = "Email not specified", AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required(ErrorMessage = "No password specified", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
