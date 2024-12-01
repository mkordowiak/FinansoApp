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


        public async Task<RepositoryResult<bool>> AddUserToGroup(int groupId, AppUser appUser)
        {
            Models.Group group;
            try
            {
                group = await _context.Groups.SingleOrDefaultAsync(x => x.Id == groupId);
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            if (group == null)
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.NotFound);
            }

            return await AddUserToGroup(group, appUser);
        }

        public async Task<RepositoryResult<bool>> AddUserToGroup(FinansoData.Models.Group group, AppUser appUser)
        {
            try
            {
                await _context.GroupUsers.AddAsync(new GroupUser
                {
                    AppUser = appUser,
                    Group = group,
                    Active = false
                });
                await _context.SaveChangesAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<bool>.Success(true);
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
