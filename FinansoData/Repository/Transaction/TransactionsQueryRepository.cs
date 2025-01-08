using FinansoData.Data;
using FinansoData.DataViewModel;
using FinansoData.DataViewModel.Transaction;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace FinansoData.Repository.Transaction
{
    public class TransactionsQueryRepository : ITransactionsQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICacheWrapper _cacheWrapper;

        public TransactionsQueryRepository(ApplicationDbContext applicationDbContext, ICacheWrapper cacheWrapper)
        {
            _applicationDbContext = applicationDbContext;
            _cacheWrapper = cacheWrapper;
        }
        
        public async Task<RepositoryResult<IEnumerable<GetTransactionsForBalance>>> GetTransactionsForBalance(int balanceId, int page, int pageSize)
        {
            string cacheDataKey = $"GetTransactionsForBalance_{balanceId}_{page}_{pageSize}";
            string cacheCountKey = $"GetTransactionsCountForBalance_{balanceId}_{page}_{pageSize}";

            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<GetTransactionsForBalance>? cacheTransactions)
                && _cacheWrapper.TryGetValue(cacheCountKey, out int cacheResultCount))
            {
                return RepositoryResult<IEnumerable<GetTransactionsForBalance>>.Success(cacheTransactions, cacheResultCount);
            }

            IQueryable<GetTransactionsForBalance> query = from transactions in _applicationDbContext.BalanceTransactions
                                                             join transactionStatus in _applicationDbContext.TransactionStatuses on transactions.TransactionStatusId equals transactionStatus.Id
                                                             join transactionType in _applicationDbContext.TransactionTypes on transactions.TransactionTypeId equals transactionType.Id
                                                             where transactions.BalanceId == balanceId
                                                             orderby transactions.TransactionDate descending
                                                             select new GetTransactionsForBalance
                                                             {
                                                                 TransactionId = transactions.Id,
                                                                 Description = transactions.Description,
                                                                 Amount = transactions.Amount,
                                                                 TransactionDate = transactions.TransactionDate,
                                                                 TransactionStatus = transactionStatus.Name,
                                                                 TransactionType = transactionType.Name
                                                             };

            int resultCount;
            List<GetTransactionsForBalance> result;
            try
            {
                result = await query.Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
                resultCount = await query.CountAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<GetTransactionsForBalance>>.Failure("Error while getting transactions", ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromSeconds(5));
            _cacheWrapper.Set(cacheCountKey, resultCount, TimeSpan.FromSeconds(5));
            return RepositoryResult<IEnumerable<GetTransactionsForBalance>>.Success(result, resultCount);
        }

        public async Task<RepositoryResult<IEnumerable<GetTransactionsForUser>>> GetTransactionsForUser(string userName, int page, int pageSize = 20)
        {
            string cacheDataKey = $"GetTransactionsForUser_{userName}_{page}_{pageSize}";
            string cacheCountKey = $"GetTransactionsCountForUser_{userName}_{page}_{pageSize}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<GetTransactionsForUser>? cacheTransactions)
                && _cacheWrapper.TryGetValue(cacheCountKey, out int cacheResultCount))
            {
                return RepositoryResult<IEnumerable<GetTransactionsForUser>>.Success(cacheTransactions, cacheResultCount);
            }

            IQueryable<GetTransactionsForUser> query = from transactions in _applicationDbContext.BalanceTransactions
                                                       join appUser in _applicationDbContext.Users on transactions.AppUserId equals appUser.Id
                                                       join transactionStatus in _applicationDbContext.TransactionStatuses on transactions.TransactionStatusId equals transactionStatus.Id
                                                       join transactionType in _applicationDbContext.TransactionTypes on transactions.TransactionTypeId equals transactionType.Id
                                                       join balance in _applicationDbContext.Balances on transactions.BalanceId equals balance.Id
                                                       join g in _applicationDbContext.Groups on balance.GroupId equals g.Id
                                                       where appUser.NormalizedUserName == userName
                                                       orderby transactions.TransactionDate descending
                                                       select new GetTransactionsForUser
                                                       {
                                                           TransactionId = transactions.Id,
                                                           GroupId = g.Id,
                                                           GroupName = g.Name,
                                                           BalanceId = balance.Id,
                                                           BalanceName = balance.Name,
                                                           Description = transactions.Description,
                                                           Amount = transactions.Amount,
                                                           TransactionDate = transactions.TransactionDate,
                                                           TransactionStatus = transactionStatus.Name,
                                                           TransactionType = transactionType.Name
                                                       };

            List<GetTransactionsForUser> result;
            int resultCount;
            try
            {
                result = await query.Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
                resultCount = await query.CountAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<GetTransactionsForUser>>.Failure("Error while getting transactions", ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromSeconds(5));
            _cacheWrapper.Set(cacheCountKey, resultCount, TimeSpan.FromSeconds(5));
            return RepositoryResult<IEnumerable<GetTransactionsForUser>>.Success(result, resultCount);
        }
    }
}
