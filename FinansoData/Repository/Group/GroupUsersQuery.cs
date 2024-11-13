using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Group
{
    public class GroupUsersQuery : IGroupUsersQuery
    {
        private readonly ApplicationDbContext _context;

        public GroupUsersQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RepositoryResult<IEnumerable<GetGroupMembersViewModel>>> GetGroupMembersAsync(int id)
        {
            IQueryable<GetGroupMembersViewModel> ownerQuery = from g in _context.Groups
                                                              join u in _context.AppUsers on g.OwnerAppUser.Id equals u.Id
                                                              where g.Id == id
                                                              select new GetGroupMembersViewModel
                                                              {
                                                                  Id = 0,
                                                                  FirstName = u.FirstName,
                                                                  LastName = u.LastName,
                                                                  IsOwner = true
                                                              };

            IQueryable<GetGroupMembersViewModel> memberQuery = from g in _context.Groups
                                                               join gu in _context.GroupUsers on g.Id equals gu.Group.Id
                                                               join u in _context.AppUsers on gu.AppUser.Id equals u.Id
                                                               where g.Id == id
                                                               select new GetGroupMembersViewModel
                                                               {
                                                                   Id = gu.Id,
                                                                   FirstName = u.FirstName,
                                                                   LastName = u.LastName,
                                                                   IsOwner = false
                                                               };


            try
            {
                List<GetGroupMembersViewModel> data = await ownerQuery.Union(memberQuery).OrderBy(x => x.FirstName).ToListAsync();

                return RepositoryResult<IEnumerable<GetGroupMembersViewModel>>.Success(data);
            }
            catch (Exception)
            {
                return RepositoryResult<IEnumerable<GetGroupMembersViewModel>>.Failure(null, ErrorType.ServerError);
            }
        }

        public async Task<RepositoryResult<DeleteGroupUserViewModel>> GetUserDeleteInfo(int groupUserId)
        {
            IQueryable<DeleteGroupUserViewModel> query = from g in _context.Groups
                                                         join gu in _context.GroupUsers on g.Id equals gu.Group.Id
                                                         join u in _context.AppUsers on gu.AppUser.Id equals u.Id
                                                         where gu.Id == groupUserId
                                                         select new DeleteGroupUserViewModel
                                                         {
                                                             GroupId = g.Id,
                                                             GroupUserId = gu.Id,
                                                             GroupName = g.Name,
                                                             UserFirstName = u.FirstName,
                                                             UserLastName = u.LastName
                                                         };

            try
            {
                DeleteGroupUserViewModel? data = await query.FirstOrDefaultAsync();
                return RepositoryResult<DeleteGroupUserViewModel>.Success(data);
            }
            catch
            {
                return RepositoryResult<DeleteGroupUserViewModel>.Failure(null, ErrorType.ServerError);

            }
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