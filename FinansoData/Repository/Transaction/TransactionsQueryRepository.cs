using FinansoData.Data;
using FinansoData.DataViewModel.Transaction;
using FinansoData.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Transaction
{
    public class TransactionsQueryRepository : ITransactionsQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly string _cacheClassName;

        public TransactionsQueryRepository(ApplicationDbContext applicationDbContext, ICacheWrapper cacheWrapper)
        {
            _applicationDbContext = applicationDbContext;
            _cacheWrapper = cacheWrapper;
            _cacheClassName = this.GetType().Name;
        }

        public async Task<RepositoryResult<IEnumerable<GetTransactionsForBalance>>> GetTransactionsForBalance(int balanceId, int page, int pageSize)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{balanceId}_{page}_{pageSize}";
            string cacheCountKey = $"{_cacheClassName}_{methodName}_count_{balanceId}_{page}_{pageSize}";

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

        public async Task<RepositoryResult<IEnumerable<GetTransactionsForUser>>> GetTransactionsCreatedByUser(string userName, int page, int pageSize = 20)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{userName}_{page}_{pageSize}";
            string cacheCountKey = $"{_cacheClassName}_{methodName}_Count_{userName}_{page}_{pageSize}";
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

        public async Task<RepositoryResult<IEnumerable<GetTransactionsForUser>>> GetTransactionsForUserUser(string userName, int page, int pageSize = 20)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{userName}_{page}_{pageSize}";
            string cacheCountKey = $"{_cacheClassName}_{methodName}_Count_{userName}_{page}_{pageSize}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<GetTransactionsForUser>? cacheTransactions)
                && _cacheWrapper.TryGetValue(cacheCountKey, out int cacheResultCount))
            {
                return RepositoryResult<IEnumerable<GetTransactionsForUser>>.Success(cacheTransactions, cacheResultCount);
            }

            IQueryable<GetTransactionsForUser> queryGroupOwner = from transaction in _applicationDbContext.BalanceTransactions
                                                                 join balance in _applicationDbContext.Balances on transaction.BalanceId equals balance.Id
                                                                 join g in _applicationDbContext.Groups on balance.GroupId equals g.Id
                                                                 join appUser in _applicationDbContext.Users on g.OwnerAppUser.Id equals appUser.Id
                                                                 join transactionStatus in _applicationDbContext.TransactionStatuses on transaction.TransactionStatusId equals transactionStatus.Id
                                                                 join transactionType in _applicationDbContext.TransactionTypes on transaction.TransactionTypeId equals transactionType.Id
                                                                 join currency in _applicationDbContext.Currencies on transaction.CurrencyId equals currency.Id
                                                                 where appUser.NormalizedUserName == userName
                                                                 select new GetTransactionsForUser
                                                                 {
                                                                     TransactionId = transaction.Id,
                                                                     GroupId = g.Id,
                                                                     GroupName = g.Name,
                                                                     BalanceId = balance.Id,
                                                                     BalanceName = balance.Name,
                                                                     Description = transaction.Description,
                                                                     Amount = transaction.Amount,
                                                                     CurrencyId = currency.Id,
                                                                     CurrencyName = currency.Name,
                                                                     CurrencyCode = currency.Code,
                                                                     TransactionDate = transaction.TransactionDate,
                                                                     TransactionStatus = transactionStatus.Name,
                                                                     TransactionType = transactionType.Name
                                                                 };

            IQueryable<GetTransactionsForUser> queryGroupMember = from transaction in _applicationDbContext.BalanceTransactions
                                                                  join balance in _applicationDbContext.Balances on transaction.BalanceId equals balance.Id
                                                                  join g in _applicationDbContext.Groups on balance.GroupId equals g.Id
                                                                  join groupUser in _applicationDbContext.GroupUsers on g.Id equals groupUser.GroupId
                                                                  join appUser in _applicationDbContext.Users on groupUser.AppUserId equals appUser.Id
                                                                  join transactionStatus in _applicationDbContext.TransactionStatuses on transaction.TransactionStatusId equals transactionStatus.Id
                                                                  join transactionType in _applicationDbContext.TransactionTypes on transaction.TransactionTypeId equals transactionType.Id
                                                                  join currency in _applicationDbContext.Currencies on transaction.CurrencyId equals currency.Id
                                                                  where appUser.NormalizedUserName == userName
                                                                  select new GetTransactionsForUser
                                                                  {
                                                                      TransactionId = transaction.Id,
                                                                      GroupId = g.Id,
                                                                      GroupName = g.Name,
                                                                      BalanceId = balance.Id,
                                                                      BalanceName = balance.Name,
                                                                      Description = transaction.Description,
                                                                      Amount = transaction.Amount,
                                                                      CurrencyId = currency.Id,
                                                                      CurrencyName = currency.Name,
                                                                      CurrencyCode = currency.Code,
                                                                      TransactionDate = transaction.TransactionDate,
                                                                      TransactionStatus = transactionStatus.Name,
                                                                      TransactionType = transactionType.Name
                                                                  };

            List<GetTransactionsForUser> result;
            int resultCount;
            try
            {
                IQueryable<GetTransactionsForUser> query = queryGroupOwner.Union(queryGroupMember).OrderByDescending(x => x.TransactionDate).AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize);
                result = await query.ToListAsync();
                resultCount = await query.CountAsync();  //throw new Exception("To zapytanie jest niepoprawne, zliczy tylko wyciagniete rekordy");
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
