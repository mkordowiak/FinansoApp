using FinansoData.Data;
using FinansoData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Account
{
    public class UserQuery : IUserQuery
    {
        private readonly ApplicationDbContext _context;
        private readonly ILookupNormalizer _lookupNormalizer;

        public UserQuery(ApplicationDbContext context, ILookupNormalizer lookupNormalizer)
        {
            _context = context;
            _lookupNormalizer = lookupNormalizer;
        }

        public async Task<RepositoryResult<AppUser?>> GetUserByEmail(string email)
        {
            AppUser? result;
            string normalizedEmail = _lookupNormalizer.NormalizeEmail(email);

            try
            {
                result = await _context.Users.Where(x => x.NormalizedEmail == normalizedEmail).AsNoTracking().SingleOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<AppUser?>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<AppUser?>.Success(result);
        }
    }
}
