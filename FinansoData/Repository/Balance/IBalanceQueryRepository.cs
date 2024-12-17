﻿using FinansoData.DataViewModel.Balance;
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

        /// <summary>
        /// Check if user has access to balance
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="balanceId"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool?>> HasUserAccessToBalance(string userName, int balanceId);

        /// <summary>
        /// Get single balance
        /// </summary>
        /// <param name="balanceId">Id of balance</param>
        /// <returns></returns>
        Task<RepositoryResult<BalanceViewModel>> GetBalance(int balanceId);
    }
}
