using FinansoData.Data;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Group
{
    public class GroupUsersManagementRepository : IGroupUsersManagementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IGroupCrudRepository _groupCrudRepository;

        public GroupUsersManagementRepository(ApplicationDbContext context, IGroupCrudRepository groupCrudRepository)
        {
            _context = context;
            _groupCrudRepository = groupCrudRepository;
        }


        public Task<RepositoryResult<bool>> AddUserToGroup(int groupId, string appUser)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryResult<bool>> RemoveAllUsersFromGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryResult<bool>> RemoveUserFromGroup(int groupId, string appUser)
        {
            throw new NotImplementedException();
        }

        public async Task<RepositoryResult<bool>> RemoveUserFromGroup(int groupUserId)
        {
            GroupUser? groupUser = await _context.GroupUsers.Where(x => x.Id == groupUserId).SingleOrDefaultAsync();

            if (groupUser == null)
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.NotFound);
            }


            try
            {
                _context.GroupUsers.Remove(groupUser);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }
            return RepositoryResult<bool>.Success(true);
        }
    }
}
