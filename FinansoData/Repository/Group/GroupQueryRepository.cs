using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Group
{
    public class GroupQueryRepository : IGroupQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;

        public GroupQueryRepository(ApplicationDbContext context, ICacheWrapper cacheWrapper)
        {
            _context = context;
            _cacheWrapper = cacheWrapper;
        }

        public async Task<RepositoryResult<Models.Group?>> GetGroupById(int groupId)
        {
            try
            {
                var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                return RepositoryResult<Models.Group>.Success(group);
            }
            catch
            {
                return RepositoryResult<Models.Group>.Failure(null, ErrorType.ServerError);
            }
        }

        public async Task<RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>> GetUserGroups(string appUser)
        {
            var cacheKey = $"GroupQueryRepository_GetUserGroups_{appUser}";
            if (_cacheWrapper.TryGetValue(cacheKey, out IEnumerable<GetUserGroupsViewModel>? cacheData))
            {
                return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Success(cacheData);
            }

            // Query to get all groups where user is owner
            IQueryable<GetUserGroupsViewModel> ownerQuery = from g in _context.Groups
                                                            join u in _context.AppUsers on g.OwnerAppUser.Id equals u.Id into gu
                                                            from u in gu.DefaultIfEmpty()
                                                            where u.UserName == appUser 
                                                            select new GetUserGroupsViewModel
                                                            {
                                                                Id = g.Id,
                                                                Name = g.Name,
                                                                IsOwner = true,
                                                                MembersCount = (from sqgu in _context.GroupUsers
                                                                                where sqgu.Group.Id == g.Id 
                                                                                && sqgu.Active == true
                                                                                select sqgu.Id).Count() + 1
                                                            };

            // Query to get all groups where user is member
            IQueryable<GetUserGroupsViewModel> memberQuery = from g in _context.Groups
                                                             join gu in _context.GroupUsers on g.Id equals gu.Group.Id
                                                             join u in _context.AppUsers on gu.AppUser.Id equals u.Id
                                                             where u.UserName == appUser && gu.Active == true
                                                             select new GetUserGroupsViewModel
                                                             {
                                                                 Id = g.Id,
                                                                 Name = g.Name,
                                                                 IsOwner = false,
                                                                 MembersCount = (from sqgu in _context.GroupUsers
                                                                                 where sqgu.Group.Id == g.Id
                                                                                 && sqgu.Active == true
                                                                                 select sqgu.Id).Count() + 1
                                                             };

            List<GetUserGroupsViewModel> data;
            try
            {
                data = await ownerQuery.Union(memberQuery).OrderBy(x => x.Name).ToListAsync();
            }
            catch (Exception)
            {
                return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, data, TimeSpan.FromSeconds(5));
            return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Success(data);
        }

        

        public async Task<RepositoryResult<bool>> IsGroupExists(int groupId)
        {
            IQueryable<Models.Group> query = from g in _context.Groups
                                             where g.Id == groupId
                                             select g;

            bool queryResult = false;
            try
            {
                queryResult = await query.AnyAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<bool>.Success(queryResult);
        }
    }
}
