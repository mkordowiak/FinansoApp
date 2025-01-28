using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Group
{
    public class GroupQueryRepository : IGroupQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly string _cacheClassName;

        public GroupQueryRepository(ApplicationDbContext context, ICacheWrapper cacheWrapper)
        {
            _context = context;
            _cacheWrapper = cacheWrapper;
            _cacheClassName = this.GetType().Name;
        }

        public async Task<RepositoryResult<Models.Group?>> GetGroupById(int groupId)
        {
            string methodName = MethodName.GetMethodName();
            string cacheKey = $"{_cacheClassName}_{methodName}_{groupId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out Models.Group? cacheData))
            {
                return RepositoryResult<Models.Group>.Success(cacheData);
            }


            Models.Group? group;
            try
            {
                group = await _context.Groups.AsNoTracking().FirstOrDefaultAsync(g => g.Id == groupId);
            }
            catch
            {
                return RepositoryResult<Models.Group>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, group, TimeSpan.FromSeconds(3));
            return RepositoryResult<Models.Group>.Success(group);
        }

        public async Task<RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>> GetUserGroups(string appUser)
        {
            string methodName = MethodName.GetMethodName();
            string cacheKey = $"{_cacheClassName}_{methodName}_{appUser}";
            if (_cacheWrapper.TryGetValue(cacheKey, out IEnumerable<GetUserGroupsViewModel>? cacheData))
            {
                return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Success(cacheData);
            }

            // Query to get all groups where user is owner
            IQueryable<GetUserGroupsViewModel> ownerQuery = from g in _context.Groups.AsNoTracking()
                                                            join u in _context.AppUsers.AsNoTracking() on g.OwnerAppUser.Id equals u.Id into gu
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
            IQueryable<GetUserGroupsViewModel> memberQuery = from g in _context.Groups.AsNoTracking()
                                                             join gu in _context.GroupUsers.AsNoTracking() on g.Id equals gu.Group.Id
                                                             join u in _context.AppUsers.AsNoTracking() on gu.AppUser.Id equals u.Id
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
            string methodName = MethodName.GetMethodName();
            string cacheKey = $"{_cacheClassName}_{methodName}_{groupId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out bool cacheData))
            {
                return RepositoryResult<bool>.Success(cacheData);
            }

            IQueryable<Models.Group> query = from g in _context.Groups
                                             where g.Id == groupId
                                             select g;

            bool queryResult = false;
            try
            {
                queryResult = await query.AsNoTracking().AnyAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, queryResult, TimeSpan.FromSeconds(3));
            return RepositoryResult<bool>.Success(queryResult);
        }
    }
}
