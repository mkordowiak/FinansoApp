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
        /// Returns true if group exists
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> IsGroupExists(int groupId);

        /// <summary>
        /// Get single Group by id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<Models.Group?>> GetGroupById(int groupId);

        /// <summary>
        /// Get group balances amount
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<double?>> GetGroupBalancesAmount(int groupId);
    }

}
