using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Helpers;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Group
{
    public class GroupUsersQuery : IGroupUsersQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly string _cacheClassName;

        public GroupUsersQuery(ApplicationDbContext context, ICacheWrapper cacheWrapper)
        {
            _context = context;
            _cacheWrapper = cacheWrapper;

            _cacheClassName = this.GetType().Name;
        }


        public async Task<RepositoryResult<IEnumerable<GetGroupInvitationsViewModel>>> GetGroupInvitations(string appUser)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{appUser}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<GetGroupInvitationsViewModel>? cacheData))
            {
                return RepositoryResult<IEnumerable<GetGroupInvitationsViewModel>>.Success(cacheData);
            }

            IQueryable<GetGroupInvitationsViewModel> query = from gu in _context.GroupUsers.AsNoTracking()
                                                             join g in _context.Groups.AsNoTracking() on gu.Group.Id equals g.Id
                                                             join u in _context.AppUsers.AsNoTracking() on gu.AppUser.Id equals u.Id
                                                             where gu.Active == false
                                                             && u.NormalizedEmail == appUser
                                                             select new GetGroupInvitationsViewModel
                                                             {
                                                                 GroupUserId = gu.Id,
                                                                 GroupName = g.Name,
                                                                 GroupOwnerFirstName = g.OwnerAppUser.FirstName,
                                                                 GroupOwnerLastName = g.OwnerAppUser.LastName,
                                                                 GroupMembersNum = g.GroupUser.Count()
                                                             };
            List<GetGroupInvitationsViewModel> result;
            try
            {
                result = await query.ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<GetGroupInvitationsViewModel>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromSeconds(30));
            return RepositoryResult<IEnumerable<GetGroupInvitationsViewModel>>.Success(result);
        }

        public async Task<RepositoryResult<IEnumerable<GetGroupMembersViewModel>>> GetGroupMembersAsync(int id, bool IncludeInvitations = true)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{id}_{IncludeInvitations}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<GetGroupMembersViewModel> cacheData))
            {
                return RepositoryResult<IEnumerable<GetGroupMembersViewModel>>.Success(cacheData);
            }


            IQueryable<GetGroupMembersViewModel> ownerQuery = from g in _context.Groups.AsNoTracking()
                                                              join u in _context.AppUsers.AsNoTracking() on g.OwnerAppUser.Id equals u.Id
                                                              where g.Id == id
                                                              select new GetGroupMembersViewModel
                                                              {
                                                                  Id = 0,
                                                                  FirstName = u.FirstName,
                                                                  LastName = u.LastName,
                                                                  IsOwner = true,
                                                                  InvitationAccepted = true
                                                              };

            IQueryable<GetGroupMembersViewModel> memberQuery = from g in _context.Groups.AsNoTracking()
                                                               join gu in _context.GroupUsers.AsNoTracking() on g.Id equals gu.Group.Id
                                                               join u in _context.AppUsers.AsNoTracking() on gu.AppUser.Id equals u.Id
                                                               where g.Id == id
                                                               && (IncludeInvitations || gu.Active)
                                                               select new GetGroupMembersViewModel
                                                               {
                                                                   Id = gu.Id,
                                                                   FirstName = u.FirstName,
                                                                   LastName = u.LastName,
                                                                   IsOwner = false,
                                                                   InvitationAccepted = gu.Active
                                                               };

            List<GetGroupMembersViewModel> data;
            try
            {
                data = await ownerQuery.Union(memberQuery).OrderBy(x => x.FirstName).ToListAsync();
            }
            catch (Exception)
            {
                return RepositoryResult<IEnumerable<GetGroupMembersViewModel>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, data, TimeSpan.FromSeconds(30));
            return RepositoryResult<IEnumerable<GetGroupMembersViewModel>>.Success(data);
        }

        public async Task<RepositoryResult<int>> GetGroupUsersCount(int groupId, bool includingGroupOwner = true)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{groupId}_{includingGroupOwner}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out int cacheData))
            {
                return RepositoryResult<int>.Success(cacheData);
            }

            IQueryable<GroupUser> query = from gu in _context.GroupUsers
                                          where gu.Group.Id == groupId
                                          select gu;

            int count;
            try
            {
                count = await query.AsNoTracking().CountAsync();
            }
            catch
            {
                return RepositoryResult<int>.Failure(null, ErrorType.ServerError);
            }

            if (includingGroupOwner)
            {
                count++;
            }

            _cacheWrapper.Set(cacheDataKey, count, TimeSpan.FromSeconds(30));
            return RepositoryResult<int>.Success(count);
        }

        public async Task<RepositoryResult<int>> GetInvitationCountForGroup(string appUser)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{appUser}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out int? cacheData))
            {
                return RepositoryResult<int>.Success((int)cacheData);
            }

            IQueryable<GroupUser> query = from gu in _context.GroupUsers
                                          join g in _context.Groups on gu.Group.Id equals g.Id
                                          where gu.AppUser.UserName == appUser && gu.Active == false
                                          select gu;

            int invitationCount;
            try
            {
                invitationCount = await query.AsNoTracking().CountAsync();
            }
            catch
            {
                return RepositoryResult<int>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, invitationCount, TimeSpan.FromSeconds(30));
            return RepositoryResult<int>.Success(invitationCount);
        }

        public async Task<RepositoryResult<DeleteGroupUserViewModel>> GetUserDeleteInfo(int groupUserId)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{groupUserId}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out DeleteGroupUserViewModel cacheData))
            {
                return RepositoryResult<DeleteGroupUserViewModel>.Success(cacheData);
            }


            IQueryable<DeleteGroupUserViewModel> query = from g in _context.Groups.AsNoTracking()
                                                         join gu in _context.GroupUsers.AsNoTracking() on g.Id equals gu.Group.Id
                                                         join u in _context.AppUsers.AsNoTracking() on gu.AppUser.Id equals u.Id
                                                         where gu.Id == groupUserId
                                                         select new DeleteGroupUserViewModel
                                                         {
                                                             GroupId = g.Id,
                                                             GroupUserId = gu.Id,
                                                             GroupName = g.Name,
                                                             UserFirstName = u.FirstName,
                                                             UserLastName = u.LastName
                                                         };
            DeleteGroupUserViewModel? data;
            try
            {
                data = await query.FirstOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<DeleteGroupUserViewModel>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, data, TimeSpan.FromSeconds(60));
            return RepositoryResult<DeleteGroupUserViewModel>.Success(data);
        }

        public async Task<RepositoryResult<GetUserMembershipInGroupViewModel>> GetUserMembershipInGroupAsync(int groupId, string appUser)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{groupId}_{appUser}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out GetUserMembershipInGroupViewModel cacheData))
            {
                return RepositoryResult<GetUserMembershipInGroupViewModel>.Success(cacheData);
            }

            GetUserMembershipInGroupViewModel output = new GetUserMembershipInGroupViewModel();

            IQueryable<Models.Group> isUserAdminQuery = from g in _context.Groups
                                                        join u in _context.AppUsers on g.OwnerAppUser.Id equals u.Id
                                                        where
                                                            g.Id == groupId
                                                            && u.UserName == appUser
                                                        select g;
            Models.Group? isUserAdmin;


            try
            {
                isUserAdmin = await isUserAdminQuery.AsNoTracking().FirstOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<GetUserMembershipInGroupViewModel>.Failure(null, ErrorType.ServerError);
            }


            if (isUserAdmin is not null)
            {
                output.IsMember = true;
                output.IsOwner = true;
                return RepositoryResult<GetUserMembershipInGroupViewModel>.Success(output);
            }


            IQueryable<GroupUser> isUserMemberQuery = from gu in _context.GroupUsers
                                                      join u in _context.AppUsers on gu.AppUser.Id equals u.Id
                                                      where
                                                                                                  gu.Group.Id == groupId
                                                                                                  && gu.Active == true
                                                                                                  && gu.AppUser.UserName == appUser
                                                      select gu;

            GroupUser? isUserMember;
            try
            {
                isUserMember = await isUserMemberQuery.AsNoTracking().FirstOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<GetUserMembershipInGroupViewModel>.Failure(null, ErrorType.ServerError);
            }


            if (isUserMember is not null)
            {
                output.IsOwner = false;
                output.IsMember = true;
                return RepositoryResult<GetUserMembershipInGroupViewModel>.Success(output);
            }

            output.IsMember = false;
            output.IsOwner = false;


            _cacheWrapper.Set(cacheDataKey, output, TimeSpan.FromSeconds(15));
            return RepositoryResult<GetUserMembershipInGroupViewModel>.Success(output);
        }

        public async Task<RepositoryResult<bool>> IsUserGroupOwner(int groupId, string appUser)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{groupId}_{appUser}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out bool cacheData))
            {
                return RepositoryResult<bool>.Success(cacheData);
            }


            IQueryable<Models.Group> query = from g in _context.Groups
                                             join a in _context.AppUsers on g.OwnerAppUser.Id equals a.Id
                                             where
                                                g.Id == groupId
                                                && a.NormalizedEmail == appUser
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


            _cacheWrapper.Set(cacheDataKey, queryResult, TimeSpan.FromMinutes(5));
            return RepositoryResult<bool>.Success(queryResult);
        }

        public async Task<RepositoryResult<bool>> IsUserInvited(int groupUserId, string appUser)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{groupUserId}_{appUser}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out bool cacheData))
            {
                return RepositoryResult<bool>.Success(cacheData);
            }
            IQueryable<GroupUser> query = from gu in _context.GroupUsers
                                          join u in _context.AppUsers on gu.AppUser.Id equals u.Id
                                          where
                                           gu.Id == groupUserId
                                           && u.NormalizedEmail == appUser
                                           && gu.Active == false
                                          select gu;

            bool result;
            try
            {
                result = await query.AsNoTracking().AnyAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromSeconds(30));
            return RepositoryResult<bool>.Success(result);
        }

    }
}