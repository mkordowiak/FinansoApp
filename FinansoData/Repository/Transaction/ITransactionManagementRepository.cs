namespace FinansoData.Repository.Transaction
{
    public interface ITransactionManagementRepository
    {
        Task<RepositoryResult<bool>> AddTransaction(decimal Amount, string? Description, int BalanceId, DateTime TransactionDate, string UserName, int TransactionTypeId, int TransactionStatusId);
    }
}
