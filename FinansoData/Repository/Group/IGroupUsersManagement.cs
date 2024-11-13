using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository.Group
{
    public interface IGroupUsersManagement
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
        /// Remove all users from group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> RemoveAllUsersFromGroup(int groupId);
    }
}
