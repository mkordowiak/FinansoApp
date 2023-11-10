using FinansoData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository
{
    public interface IAccountRepository
    {
        Task<bool> IsUserExists(string username);
        Task<AppUser?> GetUser(string username);
        Task<AppUser?> GetUserByEmail(string email);
        Task<AppUser> AppUser(AppUser user);
    }
}
