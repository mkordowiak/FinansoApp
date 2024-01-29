using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinansoData.Repository
{
    public interface IGroupRepository
    {
        /// <summary>
        /// Errors while performing operations
        /// </summary>
        IEnumerable<KeyValuePair<string, bool>> Error { get; }

        #region CRUD operations
        bool Add(Group group);
        bool Update(Group group);
        bool Delete(Group group);
        bool Save();
        Task<bool> SaveAsync();
        Task<bool> UpdateAsync(Group group);
        #endregion

        Task<bool> Add(string groupName, string appUser);
        Task<IEnumerable<GetUserGroupsViewModel>?> GetUserGroups(string appUser);

    }
}
