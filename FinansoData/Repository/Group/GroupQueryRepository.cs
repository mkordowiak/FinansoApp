using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Group
{
    public class GroupQueryRepository : IGroupQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupQueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RepositoryResult<IEnumerable<GetGroupMembersViewModel>>> GetGroupMembersAsync(int id)
        {
            var query = from g in _context.Groups
                        from gu in _context.GroupUsers
                        from u in _context.AppUsers
                        where g.Id == id
                        &&
                        (
                           g.OwnerAppUser.Id == u.Id
                           || gu.AppUser.Id == u.Id
                        )
                        select new GetGroupMembersViewModel
                        {
                            Id = (g.OwnerAppUser.Id == u.Id) ? 0 : gu.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            IsOwner = (g.OwnerAppUser.Id == u.Id) ? true : false
                        };
            
            try
            {
                List<GetGroupMembersViewModel> data = await query.ToListAsync();

                return RepositoryResult<IEnumerable<GetGroupMembersViewModel>>.Success(data);
            }
            catch (Exception)
            {
                return RepositoryResult<IEnumerable<GetGroupMembersViewModel>>.Failure(null, ErrorType.ServerError);
            }
        }

        public async Task<RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>> GetUserGroups(string appUser)
        {
            AppUser user;
            try
            {
                user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(appUser));
            }
            catch (Exception)
            {
                return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Failure(null, ErrorType.ServerError);
            }

            if (user == null)
            {
                return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Failure(null, ErrorType.NoUserFound);
            }


            IQueryable<GetUserGroupsViewModel> query = from g in _context.Groups
                                                       join gu in _context.GroupUsers on g.Id equals gu.Group.Id into guj
                                                       from gg in guj.DefaultIfEmpty()
                                                       where
                                                       (gg.AppUser == user && gg.Active == true)
                                                       || (g.OwnerAppUser == user)
                                                       select new GetUserGroupsViewModel
                                                       {
                                                           Id = g.Id,
                                                           Name = g.Name,
                                                           IsOwner = (g.OwnerAppUser == user),
                                                           MembersCount = (from gusq in _context.GroupUsers
                                                                           where gusq.Group.Id.Equals(g.Id)
                                                                           && gusq.Active == true
                                                                           select gusq.Id).Count() + 1
                                                       };

            List<GetUserGroupsViewModel> data;
            try
            {
                data = await query.ToListAsync();
            }
            catch (Exception)
            {
                return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Failure(null, ErrorType.ServerError);
            }

            return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Success(data);
        }

        public async Task<RepositoryResult<GetUserMembershipInGroupViewModel>> GetUserMembershipInGroupAsync(int groupId, string appUser)
        {
            GetUserMembershipInGroupViewModel output = new GetUserMembershipInGroupViewModel();

            IQueryable<Models.Group> isUserAdminQuery = from g in _context.Groups
                                                        join u in _context.AppUsers on g.OwnerAppUser.Id equals u.Id
                                                        where g.Id == groupId && u.UserName == appUser
                                                        select g;
            Models.Group isUserAdmin;


            try
            {
                isUserAdmin = await isUserAdminQuery.FirstOrDefaultAsync();
            }
            catch (Exception ex)
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

            GroupUser? isUserMember = await isUserMemberQuery.FirstOrDefaultAsync();

            if (isUserMember is not null)
            {
                output.IsOwner = false;
                output.IsMember = true;
                return RepositoryResult<GetUserMembershipInGroupViewModel>.Success(output);
            }

            output.IsMember = false;
            output.IsOwner = false;
            return RepositoryResult<GetUserMembershipInGroupViewModel>.Success(output);
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

        public async Task<RepositoryResult<bool>> IsUserGroupOwner(int groupId, string appUser)
        {
            IQueryable<Models.Group> query = from g in _context.Groups
                                             join a in _context.AppUsers on g.OwnerAppUser.Id equals a.Id
                                             where g.Id == groupId && a.NormalizedEmail == appUser
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
