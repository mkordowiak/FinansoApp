using FinansoData.Models;

namespace FinansoData.Repository
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Errors while performing operations
        /// </summary>
        IEnumerable<KeyValuePair<string, bool>> Error { get; }

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
