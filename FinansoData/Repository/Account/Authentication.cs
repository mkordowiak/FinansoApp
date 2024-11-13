using FinansoData.Data;
using FinansoData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FinansoData.Repository.Account
{
    public class Authentication : IAuthentication
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountErrorHandling _errorHandling;
        private readonly UserManager<AppUser> _userManager;


        public Authentication(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<RepositoryResult<AppUser?>> GetUserAsync(string username)
        {
            AppUser user;
            try
            {
                user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(username));
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<AppUser?>.Success(user);
        }

        public async Task<RepositoryResult<AppUser?>> GetUserByEmailAsync(string email)
        {
            AppUser user;
            try
            {
                user = await _context.AppUsers.FirstOrDefaultAsync(x => x.NormalizedEmail.Equals(email));

            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<AppUser?>.Success(user);
        }

        public async Task<RepositoryResult<bool>> IsUserExistsAsync(string username)
        {
            AppUser user;

            try
            {
                user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(username));
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            if (user == null)
            {
                return RepositoryResult<bool>.Success(false);
            }
            else
            {
                return RepositoryResult<bool>.Success(true);
            }
        }

        public async Task<RepositoryResult<bool>> IsUserExistsByEmailAsync(string email)
        {
            string emailNormalized = _userManager.NormalizeEmail(email);
            AppUser user;

            try
            {
                user = await _context.AppUsers
                .FirstOrDefaultAsync(x => x.NormalizedEmail == emailNormalized);
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }


            if (user == null)
            {
                return RepositoryResult<bool>.Success(false);
            }
            else
            {
                return RepositoryResult<bool>.Success(true);
            }

        }

        public async Task<RepositoryResult<AppUser?>> LoginAsync(string Email, string Password)
        {
            // Check if user exists
            AppUser? user;
            try
            {
                user = await _userManager.FindByEmailAsync(Email);
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }

            // If user does not exists
            if (user == null)
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.NoUserFound);
            }

            // Check if password marches
            bool passwordCheck;
            try
            {
                passwordCheck = await _userManager.CheckPasswordAsync(user, Password);
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }


            // If password does not matches
            if (passwordCheck == false)
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.WrongPassword);
            }

            return RepositoryResult<AppUser?>.Success(user);
        }
    }
}
