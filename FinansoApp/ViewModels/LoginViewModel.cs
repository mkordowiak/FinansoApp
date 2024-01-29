using System.ComponentModel.DataAnnotations;

namespace FinansoApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please provide correct email address")]
        [Display(Name = "Email Adreess")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
