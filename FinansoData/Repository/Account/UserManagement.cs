using FinansoData.Data;
using FinansoData.Models;
using Microsoft.AspNetCore.Identity;

namespace FinansoData.Repository.Account
{
    public class UserManagement : IUserManagement
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthentication _authentication;

        public UserManagement(ApplicationDbContext context, UserManager<AppUser> userManager, IAuthentication authentication)
        {
            _context = context;
            _userManager = userManager;
            _authentication = authentication;
        }

        private async Task<bool> Save()
        {
            int saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<RepositoryResult<AppUser?>> AddUserToRoleAdminAsync(AppUser appUser)
        {
            try
            {
                await _userManager.AddToRoleAsync(appUser, UserRoles.Admin);
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<AppUser?>.Success(appUser);
        }

        public async Task<RepositoryResult<AppUser?>> AddUserToRoleUserAsync(AppUser appUser)
        {
            try
            {
                await _userManager.AddToRoleAsync(appUser, UserRoles.User);
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<AppUser?>.Success(appUser);
        }

        public async Task<RepositoryResult<bool>> AdminSetNewPassword(AppUser user, string newPassword)
        {
            IdentityResult removePasswordResult;
            IdentityResult addPasswordResult;
            try
            {
                removePasswordResult = await _userManager.RemovePasswordAsync(user);
                addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            if (removePasswordResult == null || addPasswordResult == null)
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<bool>.Success(true);
        }

        public async Task<RepositoryResult<bool>> AdminSetNewPassword(string username, string newPassword)
        {
            RepositoryResult<AppUser?> user = await _authentication.GetUserAsync(username);

            if (user.IsSuccess == false)
            {
                return RepositoryResult<bool>.Failure(null, user.ErrorType);
            }


            if (user.Value == null)
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.NoUserFound);
            }


            RepositoryResult<bool> result = await this.AdminSetNewPassword(user.Value, newPassword);

            if (result.IsSuccess == false)
            {
                return RepositoryResult<bool>.Failure(null, result.ErrorType);
            }

            return RepositoryResult<bool>.Success(true);

        }

        public async Task<RepositoryResult<AppUser?>> CreateAppUser(string Email, string Password)
        {
            // Check if user is already exists
            RepositoryResult<AppUser?> user = await _authentication.GetUserByEmailAsync(Email);

            if (user.IsSuccess == false)
            {
                return RepositoryResult<AppUser?>.Failure(null, user.ErrorType);
            }


            // End when user is already existed
            if (user.Value != null)
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.EmailAlreadyExists);
            }


            // New user object
            AppUser newUser = new AppUser()
            {
                UserName = Email,
                Email = Email,
                EmailConfirmed = false,
                Created = DateTime.Now
            };


            // Create user
            IdentityResult userCreation;
            try
            {
                userCreation = await _userManager.CreateAsync(newUser, Password);
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }


            // When can't create user
            if (userCreation == null || userCreation.Succeeded == false)
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.RegisterError);
            }

            // Assign user role
            IdentityResult userRoleAssign;
            try
            {
                userRoleAssign = await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            }
            catch
            {
                await DeleteUserAsync(newUser);
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.AssignUserRoleError);
            }

            // If can't assing user to role
            if (userRoleAssign == null || userRoleAssign.Succeeded == false)
            {
                await DeleteUserAsync(newUser);
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.AssignUserRoleError);
            }


            return RepositoryResult<AppUser?>.Success(newUser);
        }

        public async Task<RepositoryResult<bool>> DeleteUserAsync(AppUser user)
        {
            try
            {
                await _userManager.DeleteAsync(user);
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<bool>.Success(true);
        }

        public async Task<RepositoryResult<bool>> EditUserInfo(AppUser user)
        {
            try
            {
                _context.Users.Update(user);
                bool saved = await Save();

                return RepositoryResult<bool>.Success(saved);
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }
        }
    }
}
