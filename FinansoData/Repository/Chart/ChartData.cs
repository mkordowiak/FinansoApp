using Azure;
using FinansoData.Data;
using FinansoData.DataViewModel.Transaction;
using FinansoData.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository.Chart
{
    public class ChartData : IChartData
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly string _cacheClassName;

        public ChartData(ApplicationDbContext applicationDbContext, ICacheWrapper cacheWrapper)
        {
            _applicationDbContext = applicationDbContext;
            _cacheWrapper = cacheWrapper;
            _cacheClassName = this.GetType().Name;
        }

        public Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetExpensesInCategories(int balanceId, int months = 12)
        {
            return GetTransactionsInCategories(2, balanceId, months);
        }

        public Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetIncomesInCategories(int balanceId, int months = 12)
        {
            return GetTransactionsInCategories(1, balanceId, months);
        }

        private async Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetTransactionsInCategories(int typeId, int balanceId, int months = 12)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{typeId}_{balanceId}_{months}";

            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<Tuple<string, decimal>>? cacheData))
            {
                return RepositoryResult<IEnumerable<Tuple<string, decimal>>>.Success(cacheData);
            }

            var query = from transaction in _applicationDbContext.BalanceTransactions
                        where transaction.BalanceId == balanceId
                        && transaction.TransactionTypeId == typeId
                        && transaction.TransactionDate >= DateTime.Now.AddMonths(-months)
                        group transaction by transaction.TransactionCategory.Name into g
                        select new
                        {
                            Category = g.Key,
                            Amount = g.Sum(x => x.Amount)
                        };


            IEnumerable<Tuple<string, decimal>> result;
            try
            {
                result = await query.Select(x => new Tuple<string, decimal>(x.Category, x.Amount)).AsNoTracking().ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<Tuple<string, decimal>>>.Failure("Error while getting transactions", ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromMinutes(5));
            return RepositoryResult<IEnumerable<Tuple<string, decimal>>>.Success(result);
        }
    }
}
