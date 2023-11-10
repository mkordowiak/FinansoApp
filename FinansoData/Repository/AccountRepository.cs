using FinansoData.Data;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository
{
    internal class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;


        public AccountRepository(ApplicationDbContext context)
        {
             _context = context;
        }
        public Task<AppUser> AppUser(AppUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<AppUser?> GetUser(string username)
        {
            return await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(username));
        }

        public async Task<AppUser?> GetUserByEmail(string email)
        {
            return await _context.AppUsers.FirstOrDefaultAsync(x => x.Email.Equals(email));
        }

        public async Task<bool> IsUserExists(string username)
        {
            AppUser user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(username));

            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
