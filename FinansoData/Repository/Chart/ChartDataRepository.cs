using FinansoData.Data;
using FinansoData.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Chart
{
    public class ChartDataRepository : IChartDataRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly string _cacheClassName;

        public ChartDataRepository(ApplicationDbContext applicationDbContext, ICacheWrapper cacheWrapper)
        {
            _applicationDbContext = applicationDbContext;
            _cacheWrapper = cacheWrapper;
            _cacheClassName = this.GetType().Name;
        }

        public Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetExpensesInCategories(string userName, int months = 12)
        {
            return GetTransactionsInCategories(2, userName, months);
        }

        public Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetIncomesInCategories(string userName, int months = 12)
        {
            return GetTransactionsInCategories(1, userName, months);
        }

        private async Task<RepositoryResult<IEnumerable<Tuple<string, decimal>>>> GetTransactionsInCategories(int typeId, string userName, int months = 12)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{typeId}_{userName}_{months}";

            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<Tuple<string, decimal>>? cacheData))
            {
                return RepositoryResult<IEnumerable<Tuple<string, decimal>>>.Success(cacheData);
            }

            var query = from transaction in _applicationDbContext.BalanceTransactions
                        join balance in _applicationDbContext.Balances on transaction.BalanceId equals balance.Id
                        join gr in _applicationDbContext.Groups on balance.GroupId equals gr.Id
                        join gu in _applicationDbContext.GroupUsers on gr.Id equals gu.GroupId
                        join user in _applicationDbContext.Users on gu.AppUserId equals user.Id
                        join currency in _applicationDbContext.Currencies on balance.CurrencyId equals currency.Id
                        where user.UserName == userName
                        && transaction.TransactionTypeId == typeId
                        && transaction.TransactionStatusId == 2
                        && transaction.TransactionDate >= DateTime.Now.AddMonths(-months)
                        && gu.Active == true
                        group new { transaction, currency } by transaction.TransactionCategory.Name into g
                        select new
                        {
                            Category = g.Key,
                            Amount = g.Sum(x => x.transaction.Amount * x.currency.ExchangeRate)
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

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromSeconds(10));
            return RepositoryResult<IEnumerable<Tuple<string, decimal>>>.Success(result);
        }
    }
}
