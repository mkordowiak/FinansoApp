namespace FinansoData.Repository.Transaction
{
    public interface ITransactionManagementRepository
    {
        Task<RepositoryResult<bool>> AddTransaction(decimal Amount, string? Description, int BalanceId, DateTime TransactionDate, string UserName, int TransactionTypeId, int TransactionStatusId, int TransactionCategoryId);
        Task<RepositoryResult<bool>> AddTransactionMonthlyRecurring(decimal Amount, string? Description, int BalanceId, DateTime TransactionStartDate, DateTime TransactionEndDate, string UserName, int TransactionTypeId, int TransactionCategoryId);
        Task<RepositoryResult<bool>> AddTransactionWeeklyRecurring(decimal Amount, string? Description, int BalanceId, DateTime TransactionStartDate, DateTime TransactionEndDate, string UserName, int TransactionTypeId, int TransactionCategoryId);
    }
}
