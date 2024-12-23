namespace FinansoData.Repository.Transaction
{
    public interface ITransactionManageRepository
    {
        Task<RepositoryResult<IEnumerable<Models.BalanceTransaction>?>> GetAllTransactionsForGroup(int groupId);
        Task<RepositoryResult<IEnumerable<Models.BalanceTransaction>?>> GetAllTransactionsForBalance(int balanceId);
    }
}
