using FinansoData.Models;

namespace FinansoData.Repository.Account
{
    public interface IUserManagement
    {
        Task<RepositoryResult<AppUser?>> AddUserToRoleUserAsync(AppUser appUser);
        Task<RepositoryResult<AppUser?>> AddUserToRoleAdminAsync(AppUser appUser);

        /// <summary>
        /// Create new app user
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="FirstName"></param>
        /// <returns></returns>
        Task<RepositoryResult<AppUser?>> CreateAppUser(string Email, string Password, string FirstName);
        Task<RepositoryResult<bool>> DeleteUserAsync(AppUser user);
        Task<RepositoryResult<bool>> AdminSetNewPassword(AppUser user, string newPassword);
        Task<RepositoryResult<bool>> AdminSetNewPassword(string username, string newPassword);
        /// <summary>
        /// Edit user info
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> EditUserInfo(AppUser user);
    }
}
