namespace FinansoData.Repository.Group
{
    public interface IGroupManagementRepository
    {
        /// <summary>
        /// Create new group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool?>> Add(string groupName, string appUser);

        /// <summary>
        /// Remove single user from gorup
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> DeleteGroupUser(int groupId, string appUser);

        /// <summary>
        /// Removes all users from group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> DeleteAllGroupUsers(int groupId);

        /// <summary>
        /// Remove user from group
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> DeleteGroupUser(int groupUserId);

        /// <summary>
        /// Removes group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> DeleteGroup(int groupId);
    }
}
