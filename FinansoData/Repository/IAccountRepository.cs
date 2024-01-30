using FinansoData.Models;

namespace FinansoData.Repository
{
    public interface IAccountRepository
    {
        IAccountRepositoryErrorInfo Err { get; set; }

        public class IAccountRepositoryErrorInfo
        {
            public bool DatabaseError { get; set; }
            public bool EmailAlreadyExists { get; set; }
            public bool RegisterError { get; set; }
            public bool AssignUserRoleError { get; set; }
            public bool UserNotFound { get; set; }
            public bool WrongPassword { get; set; }

        }

        Task<bool> IsUserExistsAsync(string username);
        Task<bool> IsUserExistsByEmailAsync(string email);
        Task<bool> DeleteUserAsync(AppUser user);

        Task<AppUser?> GetUserAsync(string username);
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<AppUser?> CreateAppUser(string Email, string Password);
        Task<AppUser?> LoginAsync(string Email, string Password);

        Task<AppUser?> AddUserToRoleUserAsync(AppUser appUser);
        Task<AppUser?> AddUserToRoleAdminAsync(AppUser appUser);
    }
}
