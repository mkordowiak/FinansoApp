using FinansoData.DataViewModel.Group;

namespace FinansoData.Repository.Group
{
    public interface IGroupUsersQueryRepository
    {
        /// <summary>
        /// Get all group members
        /// </summary>
        /// <param name="id"></param>
        /// <param name="IncludeInvitations"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<GetGroupMembersViewModel>>> GetGroupMembersAsync(int id, bool IncludeInvitations = true);

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

        /// <summary>
        /// If user is invited to group return true
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> IsUserInvited(int groupUserId, string appUser);

        /// <summary>
        /// Get group users count
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="includingGroupOwner">Includes group owner to count</param>
        /// <returns></returns>
        Task<RepositoryResult<int>> GetGroupUsersCount(int groupId, bool includingGroupOwner = true);
    }
}
