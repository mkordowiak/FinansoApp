using FinansoData.Models;

namespace FinansoData.Repository.Account
{
    public interface IAuthentication
    {
        Task<RepositoryResult<bool>> IsUserExistsAsync(string username);
        Task<RepositoryResult<bool>> IsUserExistsByEmailAsync(string email);
        Task<RepositoryResult<AppUser?>> GetUserAsync(string username);
        Task<RepositoryResult<AppUser?>> GetUserByEmailAsync(string email);
        Task<RepositoryResult<AppUser?>> LoginAsync(string Email, string Password);
    }
}
