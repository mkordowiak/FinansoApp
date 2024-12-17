using FinansoData.DataViewModel.Balance;

namespace FinansoData.Repository.Balance
{
    public interface IBalanceManagmentRepository
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
        Task<RepositoryResult<bool?>> SetBalanceAmount(int id, double amount);
    }
}
