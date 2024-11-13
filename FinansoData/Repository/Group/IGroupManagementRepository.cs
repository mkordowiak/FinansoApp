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
        /// Removes group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> DeleteGroup(int groupId);
    }
}
