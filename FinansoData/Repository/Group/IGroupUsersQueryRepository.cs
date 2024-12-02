using FinansoData.DataViewModel.Group;

namespace FinansoData.Repository.Group
{
    public interface IGroupUsersQueryRepository
    {
        /// <summary>
        /// Get all group members
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<GetGroupMembersViewModel>>> GetGroupMembersAsync(int id);

        /// <summary>
        /// Get user info for delete page
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        Task<RepositoryResult<DeleteGroupUserViewModel>> GetUserDeleteInfo(int groupUserId);

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
        /// Get invitation count for group
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<int>> GetInvitationCountForGroup(string appUser);

        /// <summary>
        /// Get list of all invitations for group
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<GetGroupInvitationsViewModel>>> GetGroupInvitations(string appUser);
    }
}
