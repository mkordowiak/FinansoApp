using FinansoData.Data;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FinansoData.DataViewModel.Group;
using static FinansoData.Repository.IGroupRepository;

namespace FinansoData.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;
        private IGroupRepositoryErrorInfo _igroupRepositoryErrorInfo;

        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
            _igroupRepositoryErrorInfo = new GroupRepositoryErrorInfo();
        }
        
        public IGroupRepository.IGroupRepositoryErrorInfo Error
        {
            get { return _igroupRepositoryErrorInfo; }
        }

        IGroupRepositoryErrorInfo IGroupRepository.Error => throw new NotImplementedException();


        #region CRUD operations
        public bool Add(Group group)
        {
            try
            {
                _context.Add(group);
                return Save();
            }
            catch
            {
                _igroupRepositoryErrorInfo.DatabaseError = true;
                return false;
            }
        }

        

        public bool Delete(Group group)
        {
            try
            {
                _context.Remove(group);
                return Save();
            }
            catch
            {
                _igroupRepositoryErrorInfo.DatabaseError = true;
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                int saved = _context.SaveChanges();
                return saved > 0 ? true : false;

            }
            catch
            {
                _igroupRepositoryErrorInfo.DatabaseError = true;
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                int saved = await _context.SaveChangesAsync();
                return saved > 0 ? true : false;
            }
            catch
            {
                _igroupRepositoryErrorInfo.DatabaseError = true;
                return false;
            }
        }

        public bool Update(Group group)
        {
            try
            {
                _context.Update(group);
                return Save();
            }
            catch
            {
                _igroupRepositoryErrorInfo.DatabaseError = true;
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Group group)
        {
            try
            {
                _context.Update(group);
                return await SaveAsync();
            }
            catch
            {
                _igroupRepositoryErrorInfo.DatabaseError = true;
                return false;
            }
        }

        #endregion

        public async Task<bool> Add(string groupName, string appUser)
        {
            AppUser user;
            try
            {

                user = await _context.AppUsers.FirstOrDefaultAsync(x => x.Email.Equals(appUser));

            }
            catch (Exception)
            {
                _igroupRepositoryErrorInfo.DatabaseError = true;
                return false;
            }

            if(user == null)
            {
                _igroupRepositoryErrorInfo.NoUserFoundError = true;
                return false;
            }

            // Check if user reached max group limit
            int userGroupCount = await _context.Groups.CountAsync( x => x.OwnerAppUser.Equals (user) );

            if (userGroupCount >= 10)
            {
                _igroupRepositoryErrorInfo.MaxGroupsLimitReached = true;
                return false;
            }

            Group group = new Group
            {
                Name = groupName,
                Created = DateTime.Now,
                OwnerAppUser = user
            };

            return Add(group) ? true : false;
        }

        public async Task<IEnumerable<GetUserGroupsViewModel>?> GetUserGroups(string appUser)
        {
            AppUser user;
            try
            {

                user = await _context.AppUsers.FirstOrDefaultAsync(x => x.Email.Equals(appUser));

            }
            catch (Exception)
            {
                _igroupRepositoryErrorInfo.DatabaseError = true;
                return null;
            }

            if (user == null)
            {
                _igroupRepositoryErrorInfo.NoUserFoundError = true;
                return null;
            }


            var query = from g in _context.Groups
                                      join gu in _context.GroupUsers on g.Id equals gu.Group.Id
                                      where gu.AppUser == user
                                      && gu.Active == true
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

            try
            {
                return await query.ToListAsync();
            }
            catch (Exception)
            {
                _igroupRepositoryErrorInfo.DatabaseError = true;
                return null;
            }
        }
    }
}
