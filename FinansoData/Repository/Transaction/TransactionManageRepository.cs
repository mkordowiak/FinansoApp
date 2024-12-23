using FinansoData.Data;
using FinansoData.DataViewModel.Transaction;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Transaction
{
    public class TransactionManageRepository : ITransactionManageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;

        public TransactionManageRepository(ApplicationDbContext context, ICacheWrapper cacheWrapper)
        {
            _context = context;
            _cacheWrapper = cacheWrapper;
        }


        public async Task<RepositoryResult<IEnumerable<TransactionViewModel>?>> GetAllTransactionsForBalance(int balanceId)
        {
            string cacheKey = $"TransactionManageRepository_GetAllTransactionsForBalance_{balanceId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out IEnumerable<TransactionViewModel>? cachedTransactions))
            {
                return RepositoryResult<IEnumerable<TransactionViewModel>>.Success(cachedTransactions);
            }

            IQueryable<TransactionViewModel> query = from bt in _context.BalanceTransactions
                                                     join c in _context.Currencies on bt.CurrencyId equals c.Id
                                                     join g in _context.Groups on bt.GroupId equals g.Id
                                                     join u in _context.Users on bt.AppUserId equals u.Id
                                                     join b in _context.Balances on bt.BalanceId equals b.Id
                                                     join tt in _context.TransactionTypes on bt.TransactionTypeId equals tt.Id
                                                     join ts in _context.TransactionStatuses on bt.TransactionStatusId equals ts.Id
                                                     where b.Id == balanceId
                                                     select new TransactionViewModel
                                                     {
                                                         Id = bt.Id,
                                                         Amount = bt.Amount,
                                                         TransactionDate = bt.TransactionDate,
                                                         Description = bt.Description,
                                                         User = u,
                                                         Group = g,
                                                         Balance = b,
                                                         transactionStatus = ts,
                                                         transactionType = tt,
                                                         Currency = c
                                                     };

            List<TransactionViewModel> transactions;
            try
            {
                transactions = await query.ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<TransactionViewModel>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, transactions, TimeSpan.FromSeconds(2));
            return RepositoryResult<IEnumerable<TransactionViewModel>>.Success(transactions);
        }

        public async Task<RepositoryResult<IEnumerable<TransactionViewModel>?>> GetAllTransactionsForGroup(int groupId)
        {
            string cacheKey = $"TransactionManageRepository_GetAllTransactionsForGroup_{groupId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out IEnumerable<TransactionViewModel>? cachedTransactions))
            {
                return RepositoryResult<IEnumerable<TransactionViewModel>>.Success(cachedTransactions);
            }

            IQueryable<TransactionViewModel> query = from bt in _context.BalanceTransactions
                                                     join c in _context.Currencies on bt.CurrencyId equals c.Id
                                                     join g in _context.Groups on bt.GroupId equals g.Id
                                                     join u in _context.Users on bt.AppUserId equals u.Id
                                                     join b in _context.Balances on bt.BalanceId equals b.Id
                                                     join tt in _context.TransactionTypes on bt.TransactionTypeId equals tt.Id
                                                     join ts in _context.TransactionStatuses on bt.TransactionStatusId equals ts.Id
                                                     where g.Id == groupId
                                                     select new TransactionViewModel
                                                     {
                                                         Id = bt.Id,
                                                         Amount = bt.Amount,
                                                         TransactionDate = bt.TransactionDate,
                                                         Description = bt.Description,
                                                         User = u,
                                                         Group = g,
                                                         Balance = b,
                                                         transactionStatus = ts,
                                                         transactionType = tt,
                                                         Currency = c
                                                     };

            List<TransactionViewModel> transactions;
            try
            {
                transactions = await query.ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<TransactionViewModel>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, transactions, TimeSpan.FromSeconds(2));
            return RepositoryResult<IEnumerable<TransactionViewModel>>.Success(transactions);
        }
    }
}
