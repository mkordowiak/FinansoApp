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

        public LoginViewModelErrorInfo Error { get; } = new LoginViewModelErrorInfo();

        public class LoginViewModelErrorInfo : FinansoData.Helpers.ErrorInfo
        {
            public bool InternalError { get; set; } = false;
            public bool WrongCredentials { get; set; } = false;
        }
    }
}
