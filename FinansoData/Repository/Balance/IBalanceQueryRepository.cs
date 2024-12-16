using FinansoData.DataViewModel.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository.Balance
{
    public interface IBalanceQueryRepository
    {
        /// <summary>
        /// Get list of balances for user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>IEnumerable of model</returns>
        Task<RepositoryResult<IEnumerable<BalanceViewModel>?>> GetListOfBalancesForUser(string userName);

        /// <summary>
        /// Get list of balances for group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>IEnumerable of model</returns>
        Task<RepositoryResult<IEnumerable<BalanceViewModel>?>> GetListOfBalancesForGroup(int groupId);
    }
}
