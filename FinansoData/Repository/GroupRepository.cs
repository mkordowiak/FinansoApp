//using FinansoData.Data;
//using FinansoData.DataViewModel.Group;
//using FinansoData.Models;
//using Microsoft.EntityFrameworkCore;

//namespace FinansoData.Repository
//{
//    public class GroupRepository : IGroupRepository
//    {
//        private readonly ApplicationDbContext _context;

//        public GroupRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }



//        #region CRUD operations
//        public bool Add(Group group)
//        {
//            try
//            {
//                _context.Add(group);
//                return Save();
//            }
//            catch
//            {
//                return false;
//            }
//        }


//        public bool Delete(Group group)
//        {
//            try
//            {
//                _context.Remove(group);
//                return Save();
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public bool Save()
//        {
//            try
//            {
//                int saved = _context.SaveChanges();
//                return saved > 0 ? true : false;

//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public async Task<bool> SaveAsync()
//        {
//            try
//            {
//                int saved = await _context.SaveChangesAsync();
//                return saved > 0 ? true : false;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public bool Update(Group group)
//        {
//            try
//            {
//                _context.Update(group);
//                return Save();
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public async Task<bool> UpdateAsync(Group group)
//        {
//            try
//            {
//                _context.Update(group);
//                return await SaveAsync();
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        #endregion

//        public async Task<RepositoryResult<bool?>> Add(string groupName, string appUser)
//        {
//            AppUser user;
//            try
//            {

//                user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(appUser));

//            }
//            catch (Exception)
//            {
//                return RepositoryResult<bool?>.Failure(null, ErrorType.ServerError);
//            }

//            if (user == null)
//            {
//                return RepositoryResult<bool?>.Failure(null, ErrorType.NoUserFound);
//            }

//            // Check if user reached max group limit
//            int userGroupCount = await _context.Groups.CountAsync(x => x.OwnerAppUser.Equals(user));

//            if (userGroupCount >= 10)
//            {
//                return RepositoryResult<bool?>.Failure(null, ErrorType.MaxGroupsLimitReached);
//            }

//            Group group = new Group
//            {
//                Name = groupName,
//                Created = DateTime.Now,
//                OwnerAppUser = user
//            };

//            bool result = Add(group);

//            if (result) return RepositoryResult<bool?>.Success(true);
//            else return RepositoryResult<bool?>.Failure(null, ErrorType.ServerError);
//        }

//        public async Task<RepositoryResult<IEnumerable<GetGroupMembersViewModel>>> GetGroupMembersAsync(int id)
//        {
//            IQueryable<GetGroupMembersViewModel> query = (from gu in _context.GroupUsers
//                                                          join g in _context.Groups on gu.Group.Id equals g.Id
//                                                          join u in _context.AppUsers on gu.AppUser.Id equals u.Id
//                                                          where g.Id == id
//                                                          select new GetGroupMembersViewModel
//                                                          {
//                                                              Id = gu.Id,
//                                                              FirstName = u.FirstName,
//                                                              LastName = u.LastName,
//                                                              IsOwner = false
//                                                          })
//                                                       .Union(from g in _context.Groups
//                                                              join u in _context.AppUsers on g.OwnerAppUser.Id equals u.Id
//                                                              where g.Id == id
//                                                              select new GetGroupMembersViewModel
//                                                              {
//                                                                  Id = 0,
//                                                                  FirstName = u.FirstName,
//                                                                  LastName = u.LastName,
//                                                                  IsOwner = true
//                                                              });



//            try
//            {
//                List<GetGroupMembersViewModel> data = await query.ToListAsync();
//                return RepositoryResult<IEnumerable<GetGroupMembersViewModel>>.Success(data);
//            }
//            catch (Exception)
//            {
//                return RepositoryResult<IEnumerable<GetGroupMembersViewModel>>.Failure(null, ErrorType.ServerError);
//            }

//        }

//        public async Task<RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>> GetUserGroups(string appUser)
//        {
//            AppUser user;
//            try
//            {
//                user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.Equals(appUser));
//            }
//            catch (Exception)
//            {
//                return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Failure(null, ErrorType.ServerError);
//            }

//            if (user == null)
//            {
//                return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Failure(null, ErrorType.NoUserFound);
//            }


//            IQueryable<GetUserGroupsViewModel> query = from g in _context.Groups
//                                                       join gu in _context.GroupUsers on g.Id equals gu.Group.Id into guj
//                                                       from gg in guj.DefaultIfEmpty()
//                                                       where
//                                                       (gg.AppUser == user && gg.Active == true)
//                                                       || (g.OwnerAppUser == user)
//                                                       select new GetUserGroupsViewModel
//                                                       {
//                                                           Id = g.Id,
//                                                           Name = g.Name,
//                                                           IsOwner = (g.OwnerAppUser == user),
//                                                           MembersCount = (from gusq in _context.GroupUsers
//                                                                           where gusq.Group.Id.Equals(g.Id)
//                                                                           && gusq.Active == true
//                                                                           select gusq.Id).Count()
//                                                       };

//            List<GetUserGroupsViewModel> data;
//            try
//            {
//                data = await query.ToListAsync();
//            }
//            catch (Exception)
//            {
//                return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Failure(null, ErrorType.ServerError);
//            }

//            return RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>.Success(data);
//        }

//        public async Task<RepositoryResult<bool>> IsUserGroupOwner(int GroupId, string appUser)
//        {

//            IQueryable<Models.Group> query = from g in _context.Groups
//                                             join u in _context.AppUsers on g.OwnerAppUser.Id equals u.Id
//                                             where g.Id == GroupId && u.UserName == appUser
//                                             select g;

//            Models.Group? data;

//            try
//            {
//                data = await query.FirstOrDefaultAsync();
//            }
//            catch
//            {
//                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
//            }


//            if (data is null)
//            {
//                return RepositoryResult<bool>.Success(false);
//            }
//            return RepositoryResult<bool>.Success(true);
//        }

//        public async Task<RepositoryResult<GetUserMembershipInGroupViewModel>> GetUserMembershipInGroupAsync(int GroupId, string appUser)
//        {
//            GetUserMembershipInGroupViewModel output = new GetUserMembershipInGroupViewModel();

//            IQueryable<Models.Group> isUserAdminQuery = from g in _context.Groups
//                                                        join u in _context.AppUsers on g.OwnerAppUser.Id equals u.Id
//                                                        where g.Id == GroupId && u.UserName == appUser
//                                                        select g;
//            Models.Group isUserAdmin;


//            try
//            {
//                isUserAdmin = isUserAdminQuery.FirstOrDefault();
//            }
//            catch (Exception ex)
//            {
//                return RepositoryResult<GetUserMembershipInGroupViewModel>.Failure(null, ErrorType.ServerError);
//            }


//            if (isUserAdmin is not null)
//            {
//                output.IsMember = true;
//                output.IsOwner = true;
//                return RepositoryResult<GetUserMembershipInGroupViewModel>.Success(output);
//            }


//            IQueryable<GroupUser> isUserMemberQuery = from gu in _context.GroupUsers
//                                                      join u in _context.AppUsers on gu.AppUser.Id equals u.Id
//                                                      where
//                                                        gu.Group.Id == GroupId
//                                                        && gu.Active == true
//                                                        && gu.AppUser.UserName == appUser
//                                                      select gu;

//            GroupUser? isUserMember = await isUserMemberQuery.FirstOrDefaultAsync();

//            if (isUserMember is not null)
//            {
//                output.IsOwner = false;
//                output.IsMember = true;
//                return RepositoryResult<GetUserMembershipInGroupViewModel>.Success(output);
//            }

//            output.IsMember = false;
//            output.IsOwner = false;
//            return RepositoryResult<GetUserMembershipInGroupViewModel>.Success(output);

//        }
//    }
//}
