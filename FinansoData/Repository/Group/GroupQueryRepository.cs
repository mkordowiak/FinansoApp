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
            IQueryable<GetGroupMembersViewModel> query = (from gu in _context.GroupUsers
                                                          join g in _context.Groups on gu.Group.Id equals g.Id
                                                          join u in _context.AppUsers on gu.AppUser.Id equals u.Id
                                                          where g.Id == id
                                                          select new GetGroupMembersViewModel
                                                          {
                                                              Id = gu.Id,
                                                              FirstName = u.FirstName,
                                                              LastName = u.LastName,
                                                              IsOwner = false
                                                          })
            .Union(from g in _context.Groups
                   join u in _context.AppUsers on g.OwnerAppUser.Id equals u.Id
                   where g.Id == id
                   select new GetGroupMembersViewModel
                   {
                       Id = 0,
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       IsOwner = true
                   });



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
                                                                           select gusq.Id).Count()
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
                isUserAdmin = isUserAdminQuery.FirstOrDefault();
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

        public Task<RepositoryResult<bool>> IsUserGroupOwner(int groupId, string appUser)
        {
            throw new NotImplementedException();
        }
    }
}
