using FinansoData.Models;

namespace FinansoData.Repository.Group
{
    public interface IGroupUsersManagementRepository
    {
        /// <summary>
        /// Add user to group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> AddUserToGroup(int groupId, AppUser appUser);

        /// <summary>
        /// Add user to group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> AddUserToGroup(FinansoData.Models.Group group, AppUser appUser);

        /// <summary>
        /// Remove user from group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> RemoveUserFromGroup(int groupId, string appUser);

        /// <summary>
        /// Remove user from group
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> RemoveUserFromGroup(int groupUserId);

        /// <summary>
        /// Remove all users from group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> RemoveAllUsersFromGroup(int groupId);

        /// <summary>
        /// Accept group invitation
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> AcceptGroupInvitation(int groupUserId);

        /// <summary>
        /// Reject group invitation
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> RejectGroupInvitation(int groupUserId);
    }
}
