namespace FinansoData.Repository.Transaction
{
    public interface ITransactionMetaQueryRepository
    {
        /// <summary>
        /// Get all transaction statuses
        /// </summary>
        /// <returns>Tuple:
        ///     - Id
        ///     - Name</returns>
        Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetShortListOfAllTransactionStatuses();
        /// <summary>
        /// Get all transaction types
        /// </summary>
        /// <returns>Tuple:
        ///     - Id
        ///     - Name</returns>
        Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetShortListOfAllTransactionTypes();
    }
}
