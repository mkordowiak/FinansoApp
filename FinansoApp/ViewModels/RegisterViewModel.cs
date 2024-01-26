using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinansoApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confitm password")]
        [Compare("Password", ErrorMessage ="Password mismatch")]
        public string ConfirmPassword { get; set; }
    }
}
