using FinansoData.Models;

namespace FinansoData.Repository.Account
{
    public interface IUserManagement
    {
        Task<RepositoryResult<AppUser?>> AddUserToRoleUserAsync(AppUser appUser);
        Task<RepositoryResult<AppUser?>> AddUserToRoleAdminAsync(AppUser appUser);
        Task<RepositoryResult<AppUser?>> CreateAppUser(string Email, string Password);
        Task<RepositoryResult<bool>> DeleteUserAsync(AppUser user);
        Task<RepositoryResult<bool>> AdminSetNewPassword(AppUser user, string newPassword);
        Task<RepositoryResult<bool>> AdminSetNewPassword(string username, string newPassword);
    }
}
