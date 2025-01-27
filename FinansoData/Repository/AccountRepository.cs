using FinansoData.Data;
using FinansoData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static FinansoData.Repository.IAccountRepository;

namespace FinansoData.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private IAccountRepositoryErrorInfo _iaccountRepositoryErrorInfo;

        public IAccountRepository.IAccountRepositoryErrorInfo Error
        {
            get
            {
                return _iaccountRepositoryErrorInfo;
            }
        }

        public AccountRepository(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            _iaccountRepositoryErrorInfo = new AccountRepositoryErrorInfo();
        }



        public async Task<AppUser?> GetUserAsync(string username)
        {
            try
            {
                return await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(username));
            }
            catch
            {
                _iaccountRepositoryErrorInfo.DatabaseError = true;
                return null;
            }

        }

        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            return await _context.AppUsers.FirstOrDefaultAsync(x => x.Email.Equals(email));
        }

        public async Task<bool> IsUserExistsAsync(string username)
        {
            AppUser? user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(username));

            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Task<bool> IsUserExistsByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public async Task<AppUser?> CreateAppUser(string Email, string Password)
        {
            // Check if user is already exists
            AppUser? user;
            try
            {
                user = await this.GetUserByEmailAsync(Email);
            }
            catch
            {
                _iaccountRepositoryErrorInfo.DatabaseError = true;
                return null;
            }

            // End when user is already existed
            if (user != null)
            {
                _iaccountRepositoryErrorInfo.EmailAlreadyExists = true;
                return null;
            }


            // New user object
            AppUser newUser = new AppUser()
            {
                UserName = Email,
                Email = Email,
                EmailConfirmed = false,
                CreatedAt = DateTime.Now
            };


            // Create user
            IdentityResult userCreation;
            try
            {
                userCreation = await _userManager.CreateAsync(newUser, Password);
            }
            catch
            {
                _iaccountRepositoryErrorInfo.DatabaseError = true;
                return null;
            }


            // When can't create user
            if (userCreation == null || userCreation.Succeeded == false)
            {
                _iaccountRepositoryErrorInfo.RegisterError = true;
                return null;
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
                _iaccountRepositoryErrorInfo.AssignUserRoleError = true;
                return null;
            }

            // If can't assing user to role
            if (userRoleAssign == null || userRoleAssign.Succeeded == false)
            {
                await DeleteUserAsync(newUser);
                _iaccountRepositoryErrorInfo.AssignUserRoleError = true;
                return null;
            }


            return newUser;
        }

        public async Task<AppUser?> AddUserToRoleUserAsync(AppUser appUser)
        {
            await _userManager.AddToRoleAsync(appUser, UserRoles.User);
            return appUser;
        }

        public async Task<AppUser?> AddUserToRoleAdminAsync(AppUser appUser)
        {
            await _userManager.AddToRoleAsync(appUser, UserRoles.Admin);
            return appUser;
        }

        public async Task<bool> DeleteUserAsync(AppUser user)
        {
            await _userManager.DeleteAsync(user);
            return true;
        }

        public async Task<AppUser?> LoginAsync(string Email, string Password)
        {
            // Check if user exists
            AppUser? user;
            try
            {
                user = await _userManager.FindByEmailAsync(Email);
            }
            catch
            {
                _iaccountRepositoryErrorInfo.DatabaseError = true;
                return null;
            }

            // If user does not exists
            if (user == null)
            {
                _iaccountRepositoryErrorInfo.UserNotFound = true;
                return null;
            }

            // Check if password marches
            bool passwordCheck;
            try
            {
                passwordCheck = await _userManager.CheckPasswordAsync(user, Password);
            }
            catch
            {
                _iaccountRepositoryErrorInfo.DatabaseError = true;
                return null;
            }


            // If password does not matches
            if (passwordCheck == false)
            {
                _iaccountRepositoryErrorInfo.WrongPassword = true;
                return null;
            }

            return user;

        }

        public async Task<bool?> AdminSetNewPassword(AppUser user, string newPassword)
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
                _iaccountRepositoryErrorInfo.DatabaseError = true;
                return null;
            }

            if (removePasswordResult == null || addPasswordResult == null)
            {
                _iaccountRepositoryErrorInfo.DatabaseError = true;
                return null;
            }

            return true;

        }

        public async Task<bool?> AdminSetNewPassword(string username, string newPassword)
        {
            AppUser? user;
            try
            {
                user = await this.GetUserAsync(username);
            }
            catch (Exception ex)
            {
                throw ex;
            }



            if (user == null)
            {
                return false;
            }

            return await this.AdminSetNewPassword(user, newPassword);
        }
    }
}
