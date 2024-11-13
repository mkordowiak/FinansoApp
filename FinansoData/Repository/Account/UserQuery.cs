using FinansoData.Data;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Account
{
    public class UserQuery : IUserQuery
    {
        private readonly ApplicationDbContext _context;

        public UserQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RepositoryResult<AppUser?>> GetUserByEmail(string email)
        {
            AppUser? result;
            try
            {
                result = await _context.Users.Where(x => x.Email == email).SingleOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<AppUser?>.Success(result);
        }
    }
}
