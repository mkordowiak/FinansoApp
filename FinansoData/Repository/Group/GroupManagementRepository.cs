using FinansoData.Data;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Group
{
    public class GroupManagementRepository : IGroupManagementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IGroupCrudRepository _groupCrudRepository;

        public GroupManagementRepository(ApplicationDbContext context, IGroupCrudRepository groupCrudRepository)
        {
            _context = context;
            _groupCrudRepository = groupCrudRepository;
        }

        public async Task<RepositoryResult<bool?>> Add(string groupName, string appUser)
        {
            AppUser user;
            try
            {
                user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(appUser));
            }
            catch (Exception)
            {
                return RepositoryResult<bool?>.Failure(null, ErrorType.ServerError);
            }

            if (user == null)
            {
                return RepositoryResult<bool?>.Failure(null, ErrorType.NoUserFound);
            }

            // Check if user reached max group limit
            int userGroupCount = await _context.Groups.CountAsync(x => x.OwnerAppUser.Equals(user));

            if (userGroupCount >= 10)
            {
                return RepositoryResult<bool?>.Failure(null, ErrorType.MaxGroupsLimitReached);
            }

            Models.Group group = new Models.Group
            {
                Name = groupName,
                Created = DateTime.Now,
                OwnerAppUser = user
            };

            bool result = _groupCrudRepository.Add(group);

            if (result) return RepositoryResult<bool?>.Success(true);
            else return RepositoryResult<bool?>.Failure(null, ErrorType.ServerError);
        }
    }
}
