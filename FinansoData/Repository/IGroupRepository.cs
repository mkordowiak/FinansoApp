﻿using FinansoData.DataViewModel.Group;
using FinansoData.Models;


namespace FinansoData.Repository
{
    public interface IGroupRepository
    {
        IGroupRepositoryErrorInfo Error { get; }
        public interface IGroupRepositoryErrorInfo
        {
            bool DatabaseError { get; set; }
            bool NoUserFoundError { get; set; }
            bool MaxGroupsLimitReached { get; set; }

        }

        #region CRUD operations
        bool Add(Group group);
        bool Update(Group group);
        bool Delete(Group group);
        bool Save();
        Task<bool> SaveAsync();
        Task<bool> UpdateAsync(Group group);
        #endregion

        Task<bool> Add(string groupName, string appUser);
        Task<IEnumerable<GetUserGroupsViewModel>?> GetUserGroups(string appUser);
        Task<IEnumerable<GetGroupMembersViewModel>> GetGroupMembersAsync(int id);
        Task<bool> IsUserGroupOwner(int GroupId, string appUser);
        Task<GetUserMembershipInGroupViewModel> GetUserMembershipInGroupAsync(int GroupId, string appUser);

    }

    public class GroupRepositoryErrorInfo : IGroupRepository.IGroupRepositoryErrorInfo
    {
        public bool DatabaseError { get; set; }
        public bool NoUserFoundError { get; set; }
        public bool MaxGroupsLimitReached { get; set; }

    }
}
