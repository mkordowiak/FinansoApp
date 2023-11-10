using FinansoData.Data;
using FinansoData.Models;
using Microsoft.AspNetCore.Identity;

namespace FinansoData.BLL
{
    public class AccountBLL : IAccountBLL
    {
        private readonly UserManager<AppUser> _userManager;
        public AccountBLL(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUser?> AddUserToRoleAdminAsync(AppUser appUser)
        {
            await _userManager.AddToRoleAsync(appUser, UserRoles.Admin);
            return appUser;
        }

        public async Task<AppUser?> AddUserToRoleUserAsync(AppUser appUser)
        {
            await _userManager.AddToRoleAsync(appUser, UserRoles.User);
            return appUser;
        }

        public async Task<AppUser?> CreateAdminAsync(string username, string email, string password)
        {
            AppUser appUser = await this.CreateNewUserAsync(username, email, password);
            if (appUser == null)
            {
                return null;
            }

            await this.AddUserToRoleAdminAsync(appUser);
            return appUser;
        }

        public async Task<AppUser?> CreateNewUserAsync(string username, string email, string password)
        {
            // Checks if user exists
            if (await this.IsUserExistsByEmailAsync(email) == true)
            {
                return null;
            }

            AppUser appUser = new AppUser()
            {
                Email = email,
                UserName = username,
                Created = DateTime.Now

            };

            IdentityResult newUserResponse = await _userManager.CreateAsync(appUser, password);
            if (!newUserResponse.Succeeded)
            {
                return null;
            }
            return appUser;
        }

        public async Task<AppUser?> CreateUserAsync(string username, string email, string password)
        {
            AppUser appUser = await this.CreateNewUserAsync(username, email, password);
            if (appUser == null)
            {
                return null;
            }

            await this.AddUserToRoleUserAsync(appUser);
            return appUser;
        }

        public async Task<bool> IsUserExistsAsync(string username)
        {
            if (await _userManager.FindByNameAsync(username) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> IsUserExistsByEmailAsync(string email)
        {
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
