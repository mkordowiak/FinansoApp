using FinansoData.DataViewModel.Transaction;

namespace FinansoData.Repository.Transaction
{
    public interface ITransactionsQueryRepository
    {
        /// <summary>
        /// Get all transactions for balance
        /// </summary>
        /// <param name="balanceId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<GetTransactionsForBalance>>> GetTransactionsForBalance(int balanceId, int page, int pageSize = 20);

        /// <summary>
        /// Get all transactions for user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<GetTransactionsForUser>>> GetTransactionsCreatedByUser(string userName, int page, int pageSize = 20);

        /// <summary>
        /// Get all transactions for user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<GetTransactionsForUser>>> GetTransactionsForUserUser(string userName, int pageNumber, int pageSize = 20);

    }
}
