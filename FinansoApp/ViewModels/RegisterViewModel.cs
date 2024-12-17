using System.ComponentModel.DataAnnotations;

namespace FinansoApp.ViewModels
{
    public class RegisterViewModel
    {
        /// <summary>
        /// First name
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long")]
        [MaxLength(50, ErrorMessage = "Name must be at most 50 characters long")]
        public string Name { get; set; }

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
        [Compare("Password", ErrorMessage = "Password mismatch")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Errors
        /// </summary>
        public RegisterViewModelErrorInfo Error { get; } = new RegisterViewModelErrorInfo();

        public class RegisterViewModelErrorInfo : Helpers.ErrorInfo
        {
            public bool CreateUserError { get; set; }
            public bool AlreadyExists { get; set; }
        }
    }
}
