namespace FinansoData.Repository.Balance
{
    public interface IBalanceSumAmount
    {
        /// <summary>
        /// Get sum of all balances for user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<RepositoryResult<decimal?>> GetBalancesSumAmountForUser(string userName);

        /// <summary>
        /// Get group balances amount
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<decimal?>> GetGroupBalancesAmount(int groupId);
    }
}
