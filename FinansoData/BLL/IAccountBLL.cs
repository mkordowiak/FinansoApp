using FinansoData.Models;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.BLL
{
    public interface IAccountBLL
    {
        bool? RegisterAlreadyExists { get; }
        bool? RegisterError { get; }
        Task<bool> IsUserExistsAsync(string username);
        Task<bool> IsUserExistsByEmailAsync(string email);
        Task<AppUser?> CreateNewUserAsync(string username, string email, string password);
        Task<AppUser?> AddUserToRoleUserAsync(AppUser appUser);
        Task<AppUser?> AddUserToRoleAdminAsync(AppUser appUser);
        Task<AppUser?> CreateUserAsync(string username, string email, string password);
        Task<AppUser?> CreateAdminAsync(string username, string email, string password);
        Task<AppUser?> RegisterNewUserAsync(string Email, string Password);
    }
}
