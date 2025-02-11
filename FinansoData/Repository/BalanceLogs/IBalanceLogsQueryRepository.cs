

using FinansoData.DataViewModel.BalanceLog;
using FinansoData.Models;

namespace FinansoData.Repository.BalanceLogs
{
    public interface IBalanceLogsQueryRepository
    {
        Task<RepositoryResult<IEnumerable<BalanceLogAverage>>> GetBalanceLogs(int balanceId);
    }
}
