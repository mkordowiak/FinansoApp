using FinansoData.DataViewModel.Group;

namespace FinansoData.Repository.Group
{
    public interface IGroupQueryRepository
    {
        Task<RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>> GetUserGroups(string appUser);
        Task<RepositoryResult<IEnumerable<GetGroupMembersViewModel>>> GetGroupMembersAsync(int id);
        Task<RepositoryResult<bool>> IsUserGroupOwner(int groupId, string appUser);
        Task<RepositoryResult<GetUserMembershipInGroupViewModel>> GetUserMembershipInGroupAsync(int groupId, string appUser);
    }

}
