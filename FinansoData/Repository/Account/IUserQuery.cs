using FinansoData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository.Account
{
    public interface IUserQuery
    {
        /// <summary>
        /// Returns user by email, null if not found
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<RepositoryResult<AppUser?>> GetUserByEmail(string email);
    }
}
