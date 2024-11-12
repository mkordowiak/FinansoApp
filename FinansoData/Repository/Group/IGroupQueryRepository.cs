using FinansoData.DataViewModel.Group;

namespace FinansoData.Repository.Group
{
    public interface IGroupQueryRepository
    {

        /// <summary>
        /// Get all groups for user
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<GetUserGroupsViewModel>?>> GetUserGroups(string appUser);

        /// <summary>
        /// Get all group members
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<GetGroupMembersViewModel>>> GetGroupMembersAsync(int id);

        /// <summary>
        /// If user is group owner
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> IsUserGroupOwner(int groupId, string appUser);

        /// <summary>
        /// User group membership
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="appUser"></param>
        /// <returns>
        /// IsOwner
        /// IsMember
        /// </returns>
        Task<RepositoryResult<GetUserMembershipInGroupViewModel>> GetUserMembershipInGroupAsync(int groupId, string appUser);
        
        /// <summary>
        /// Returns true if group exists
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> IsGroupExists(int groupId);

        /// <summary>
        /// Get user info for delete page
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        Task<RepositoryResult<DeleteGroupUserViewModel>> GetUserDeleteInfo(int groupUserId);
    }

}
