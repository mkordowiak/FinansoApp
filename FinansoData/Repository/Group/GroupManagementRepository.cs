﻿using FinansoData.Data;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Group
{
    public class GroupManagementRepository : IGroupManagementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IGroupCrudRepository _groupCrudRepository;

        public GroupManagementRepository(ApplicationDbContext context, IGroupCrudRepository groupCrudRepository)
        {
            _context = context;
            _groupCrudRepository = groupCrudRepository;
        }

        public async Task<RepositoryResult<bool?>> Add(string groupName, string appUser)
        {
            AppUser? user;
            try
            {
                user = await _context.AppUsers
                    .FirstOrDefaultAsync(x => string.Equals(x.NormalizedUserName, appUser));
            }
            catch (Exception)
            {
                return RepositoryResult<bool?>.Failure(null, ErrorType.ServerError);
            }

            if (user == null)
            {
                return RepositoryResult<bool?>.Failure(null, ErrorType.NoUserFound);
            }

            // Check if user reached max group limit
            int userGroupCount = await _context.Groups.CountAsync(x => x.OwnerAppUser.Equals(user));

            if (userGroupCount >= 10)
            {
                return RepositoryResult<bool?>.Failure(null, ErrorType.MaxGroupsLimitReached);
            }

            Models.Group group = new Models.Group
            {
                Name = groupName,
                CreatedAt = DateTime.Now,
                OwnerAppUser = user
            };

            try
            {
                await _context.Groups.AddAsync(group);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return RepositoryResult<bool?>.Failure(null, ErrorType.ServerError);
            }

            GroupUser groupUser = new GroupUser
            {
                Group = group,
                AppUser = user,
                Active = true,
                CreatedAt = DateTime.Now
            };

            try
            {
                await _context.GroupUsers.AddAsync(groupUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return RepositoryResult<bool?>.Failure(null, ErrorType.ServerError);
            }


            return RepositoryResult<bool?>.Success(true);
        }


        public async Task<RepositoryResult<bool>> DeleteGroup(int groupId)
        {
            Models.Group? group = new Models.Group();
            List<GroupUser> groupUsers = new List<GroupUser>();
            try
            {
                group = await _context.Groups
                                      .SingleOrDefaultAsync(g => g.Id == groupId);
                groupUsers = await _context.GroupUsers
                        .Where(gu => gu.Group.Id == groupId)
                                        .ToListAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }

            if (group == null)
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.NotFound);
            }

            try
            {
                _context.GroupUsers.RemoveRange(groupUsers);
                _context.Groups.Remove(group);
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
