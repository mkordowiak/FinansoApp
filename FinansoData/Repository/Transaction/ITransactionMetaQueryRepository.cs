namespace FinansoData.Repository.Transaction
{
    public interface ITransactionMetaQueryRepository
    {
        /// <summary>
        /// Get all transaction statuses
        /// </summary>
        /// <returns>IEnumerable Tuple:\n
        ///     - Id\n
        ///     - Name</returns>
        Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetShortListOfAllTransactionStatuses();

        /// <summary>
        /// Get all transaction types
        /// </summary>
        /// <returns>IEnumerable Tuple:\n
        ///     - Id\n
        ///     - Name</returns>
        Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetShortListOfAllTransactionTypes();

        /// <summary>
        /// Get all transaction income categories
        /// </summary>
        /// <returns>IEnumerable tuple:\n
        ///     - Id\n
        ///     - Name</returns>
        Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetTransactionIncomeCategories();

        /// <summary>
        /// Get all transaction expense categories
        /// </summary>
        /// <returns>IEnumerable tuple:\n
        ///     - Id\n
        ///     - Name</returns>
        Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetTransactionExpenseCategories();
    }
}
