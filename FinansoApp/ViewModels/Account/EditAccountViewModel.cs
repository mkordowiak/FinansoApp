using System.ComponentModel.DataAnnotations;

namespace FinansoApp.ViewModels.Account
{
    public class EditAccountViewModel
    {
        [Required]
        [MaxLength(128)]
        [MinLength(2)]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ \'-]+$", ErrorMessage = "First name can only contain letters, spaces, dashes and apostrophes.")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(128)]
        [MinLength(2)]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ \'-]+$", ErrorMessage = "Last name can only contain letters, spaces, dashes and apostrophes.")]
        public string LastName { get; set; }

        [MaxLength(128)]
        [MinLength(1)]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ \'-]*$", ErrorMessage = "Nickname can only contain letters, spaces, dashes and apostrophes.")]
        public string? Nickname { get; set; }

        public EditAccountErrorInfo Error { get; set; } = new EditAccountErrorInfo();

        public class EditAccountErrorInfo : Helpers.ErrorInfo
        {
            public bool InternalError { get; set; }
            public bool UserNotFound { get; set; }
        }
    }
}
