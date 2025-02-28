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
#if DEBUG
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters long")]
#else
        [MinLength(7, ErrorMessage = "Password must be at least 7 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter and one number")]
#endif
        public string Password { get; set; }

        [Required(ErrorMessage = "You must confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
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
