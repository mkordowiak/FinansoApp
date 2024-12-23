namespace FinansoData.Repository.TransactionStatus
{
    public interface ITransactionStatusQueryRepository
    {
        Task<List<Models.TransactionStatus>> GetAllTransactionStatus();
        Task<Models.TransactionStatus> GetTransactionStatusById(int id);
        Task<Models.TransactionStatus> GetTransactionStatusByName(string name);
    }
}
