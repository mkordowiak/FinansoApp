using FinansoData.DataViewModel.Transaction;

namespace FinansoData.Repository.Transaction
{
    public interface ITransactionManageRepository
    {
        /// <summary>
        /// Get all transactions for a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<TransactionViewModel>?>> GetAllTransactionsForGroup(int groupId);

        /// <summary>
        /// Get all transactions for a balance
        /// </summary>
        /// <param name="balanceId"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<TransactionViewModel>?>> GetAllTransactionsForBalance(int balanceId);
    }
}
