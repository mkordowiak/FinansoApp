using FinansoData.DataViewModel.Group;
using FinansoData.Models;


namespace FinansoData.Repository
{
    public interface IGroupRepository
    {
        #region CRUD operations
        bool Add(Group group);
        bool Update(Group group);
        bool Delete(Group group);
        bool Save();
        Task<bool> SaveAsync();
        Task<bool> UpdateAsync(Group group);
        #endregion

        Task<RepositoryResult<bool?>> Add(string groupName, string appUser);
        Task<RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>> GetUserGroups(string appUser);
        Task<RepositoryResult<IEnumerable<GetGroupMembersViewModel>>> GetGroupMembersAsync(int id);
        Task<RepositoryResult<bool>> IsUserGroupOwner(int GroupId, string appUser);
        Task<RepositoryResult<GetUserMembershipInGroupViewModel>> GetUserMembershipInGroupAsync(int GroupId, string appUser);

    }
}
