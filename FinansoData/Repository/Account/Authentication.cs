using FinansoData.Data;
using FinansoData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Identity.Client;

namespace FinansoData.Repository.Account
{
    public class Authentication : IAuthentication
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILookupNormalizer _lookupNormalizer;


        public Authentication(ApplicationDbContext context, UserManager<AppUser> userManager, ILookupNormalizer lookupNormalizer)
        {
            _context = context;
            _userManager = userManager;
            _lookupNormalizer = lookupNormalizer;
        }

        public async Task<RepositoryResult<AppUser?>> GetUserAsync(string username)
        {
            string normalizedUsername = _lookupNormalizer.NormalizeName(username);
            AppUser user;

            try
            {
                user = await _context.AppUsers.SingleOrDefaultAsync(x => x.NormalizedUserName.Equals(normalizedUsername));
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<AppUser?>.Success(user);
        }

        public async Task<RepositoryResult<AppUser?>> GetUserByEmailAsync(string email)
        {
            string normalizedEmail = _lookupNormalizer.NormalizeEmail(email);
            AppUser user;

            try
            {
                user = await _context.AppUsers.SingleOrDefaultAsync(x => x.NormalizedEmail.Equals(normalizedEmail));
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<AppUser?>.Success(user);
        }

        public async Task<RepositoryResult<bool>> IsUserExistsAsync(string username)
        {
            string normalizedUsername = _lookupNormalizer.NormalizeName(username);
            AppUser user;

            try
            {
                user = await _context.AppUsers.SingleOrDefaultAsync(x => x.NormalizedUserName.Equals(normalizedUsername));
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
                .SingleOrDefaultAsync(x => x.NormalizedEmail == emailNormalized);
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
            string normalizedEmail = _lookupNormalizer.NormalizeEmail(Email);

            // Check if user exists
            AppUser? user;
            try
            {
                
                //user = await _userManager.FindByEmailAsync(normalizedEmail);
                user = await _context.AppUsers.SingleOrDefaultAsync(x => x.NormalizedUserName.Equals(normalizedEmail));
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
