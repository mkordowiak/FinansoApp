namespace FinansoData.Repository.Chart
{
    public interface IChartDataRepository
    {
        /// <summary>
        /// Get list of grouped expenses
        /// </summary>
        /// <param name="balanceId"></param>
        /// <param name="months"></param>
        /// <returns>IEnumerable<tuple>
        ///     - Label
        ///     - Value
        /// </tuple></returns>
        Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetExpensesInCategories(string userName, int months = 12);

        /// <summary>
        /// Get list of grouped incomes
        /// </summary>
        /// <param name="balanceId"></param>
        /// <param name="months"></param>
        /// <returns>IEnumerable<tuple>
        ///     - Label
        ///     - Valie
        /// </tuple></returns>
        Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetIncomesInCategories(string userName, int months = 12);
    }
}
