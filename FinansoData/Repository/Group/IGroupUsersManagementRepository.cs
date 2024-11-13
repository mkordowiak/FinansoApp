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
        Task<RepositoryResult<bool>> AddUserToGroup(int groupId, string appUser);

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
    }
}
