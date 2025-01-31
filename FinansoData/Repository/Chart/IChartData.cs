using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository.Chart
{
    public interface IChartData
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
        Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetExpensesInCategories(int balanceId, int months = 12);

        /// <summary>
        /// Get list of grouped incomes
        /// </summary>
        /// <param name="balanceId"></param>
        /// <param name="months"></param>
        /// <returns>IEnumerable<tuple>
        ///     - Label
        ///     - Valie
        /// </tuple></returns>
        Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetIncomesInCategories(int balanceId, int months = 12);


    }
}
