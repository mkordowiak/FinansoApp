using System.ComponentModel.DataAnnotations;

namespace FinansoApp.ViewModels
{
    public class AddGroupUserViemModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public int GroupId { get; set; }
        public AddGroupUserErrorInfo Error { get; set; } = new AddGroupUserErrorInfo();

        public class AddGroupUserErrorInfo : Helpers.ErrorInfo
        {
            public bool UserNotFound { get; set; }
            public bool UserAlreadyInGroup { get; set; }
            public bool InternalError { get; set; }
            public bool MaxGroupUsersLimitReached { get; set; }
            public bool CantAddYourself { get; set; }
        }
    }
}
