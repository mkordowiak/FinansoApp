using FinansoData.Models;

namespace FinansoData.Repository
{
    public interface IAccountRepository
    {
        IAccountRepositoryErrorInfo Error { get; }

        public interface IAccountRepositoryErrorInfo
        {
            bool DatabaseError { get; set; }
            bool EmailAlreadyExists { get; set; }
            bool RegisterError { get; set; }
            bool AssignUserRoleError { get; set; }
            public bool UserNotFound { get; set; }
            bool WrongPassword { get; set; }

        }



        Task<bool> IsUserExistsAsync(string username);
        Task<bool> IsUserExistsByEmailAsync(string email);
        Task<bool> DeleteUserAsync(AppUser user);
        
        Task<bool?> AdminSetNewPassword(AppUser user, string newPassword);
        Task<bool?> AdminSetNewPassword(string username, string newPassword);


        Task<AppUser?> GetUserAsync(string username);
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<AppUser?> CreateAppUser(string Email, string Password);
        Task<AppUser?> LoginAsync(string Email, string Password);

        Task<AppUser?> AddUserToRoleUserAsync(AppUser appUser);
        Task<AppUser?> AddUserToRoleAdminAsync(AppUser appUser);
    }

    public class AccountRepositoryErrorInfo : IAccountRepository.IAccountRepositoryErrorInfo
    {
        public bool DatabaseError { get; set; }
        public bool EmailAlreadyExists { get; set; }
        public bool RegisterError { get; set; }
        public bool AssignUserRoleError { get; set; }
        public bool UserNotFound { get; set; }
        public bool WrongPassword { get; set; }
    }
}
