﻿using FinansoData.DataViewModel.Balance;

namespace FinansoData.Repository.Balance
{
    public interface IBalanceManagementRepository
    {
        /// <summary>
        /// Add new balance
        /// </summary>
        /// <param name="balance"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> AddBalance(BalanceViewModel balance);

        /// <summary>
        /// Update existing balance
        /// </summary>
        /// <param name="balance"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> UpdateBalance(BalanceViewModel balance);

        /// <summary>
        /// Delete balance by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> DeleteBalance(int id);

        /// <summary>
        /// Set balance money amount
        /// </summary>
        /// <param name="id">balance ID</param>
        /// <param name="amount">Money amount</param>
        /// <returns></returns>
        Task<RepositoryResult<bool?>> SetBalanceAmount(int id, decimal amount);

        /// <summary>
        /// Add or subtract from balance
        /// </summary>
        /// <param name="balanceId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> AddToBalanceAmount(int balanceId, decimal amount);
    }
}
