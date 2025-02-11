using FinansoData.Data;
using FinansoData.DataViewModel.BalanceLog;
using FinansoData.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.BalanceLogs
{
    public class BalanceLogsQueryRepository : IBalanceLogsQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly string _cacheClassName;

        public BalanceLogsQueryRepository(ApplicationDbContext context, ICacheWrapper cacheWrapper)
        {
            _context = context;
            _cacheWrapper = cacheWrapper;
            _cacheClassName = this.GetType().Name;
        }
        public async Task<RepositoryResult<IEnumerable<BalanceLogAverage>>> GetBalanceLogs(int balanceId)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{balanceId}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<BalanceLogAverage>? cacheBalanceLogs))
            {
                return RepositoryResult<IEnumerable<BalanceLogAverage>>.Success(cacheBalanceLogs);
            }

            IQueryable<BalanceLogAverage> query = from log in _context.BalanceLogs
                                                  where
                                                      log.BalanceId == balanceId
                                                      && log.Date >= DateTime.Now.AddYears(-1)
                                                  group log by new
                                                  {
                                                      log.Date.Year,
                                                      log.Date.Month
                                                  } into g
                                                  select new BalanceLogAverage
                                                  {
                                                      Year = g.Key.Year,
                                                      Month = g.Key.Month,
                                                      Average = g.Average(x => x.Amount)
                                                  };

            List<BalanceLogAverage> result;

            try
            {
                result = await query.OrderBy(query => query.Year).ThenBy(query => query.Month).ToListAsync();
            }
            catch (Exception ex)
            {
                return RepositoryResult<IEnumerable<BalanceLogAverage>>.Failure(ex.Message, ErrorType.ServerError);
            }

            return RepositoryResult<IEnumerable<BalanceLogAverage>>.Success(result);

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromMinutes(30));
            return RepositoryResult<IEnumerable<BalanceLogAverage>>.Success(result);
        }
    }
}
