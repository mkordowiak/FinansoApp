using FinansoData.Data;
using FinansoData.DataViewModel.Currency;
using FinansoData.DataViewModel.Transaction;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public async Task<RepositoryResult<IEnumerable<Models.BalanceTransaction>?>> GetAllTransactionsForBalance(int balanceId)
        {
            string cacheKey = $"TransactionManageRepository_GetAllTransactionsForBalance_{balanceId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out IEnumerable<BalanceTransaction>? cachedTransactions))
            {
                return RepositoryResult<IEnumerable<BalanceTransaction>>.Success(cachedTransactions);
            }


            var query = from t in _context.BalanceTransactions
                        where t.BalanceId == balanceId
                        select t;

            List<BalanceTransaction> transactions;
            try
            {
                transactions = await query.ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<BalanceTransaction>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, transactions, TimeSpan.FromSeconds(2));
            return RepositoryResult<IEnumerable<BalanceTransaction>>.Success(transactions);
        }

        public async Task<RepositoryResult<IEnumerable<Models.BalanceTransaction>?>> GetAllTransactionsForGroup(int groupId)
        {
            string cacheKey = $"TransactionManageRepository_GetAllTransactionsForGroup_{groupId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out IEnumerable<BalanceTransaction>? cachedTransactions))
            {
                return RepositoryResult<IEnumerable<BalanceTransaction>>.Success(cachedTransactions);
            }


            var query = from t in _context.BalanceTransactions
                        where t.GroupId == groupId
                        select t;

            var q2 = from bt in _context.BalanceTransactions
                     join c in _context.Currencies on bt.CurrencyId equals c.Id
                     join g in _context.Groups on bt.GroupId equals g.Id
                     join u in _context.Users on bt.AppUserId equals u.Id
                     join b in _context.Balances on bt.BalanceId equals b.Id
                     join tt in _context.TransactionTypes on bt.TransactionTypeId equals tt.Id
                     join ts in _context.TransactionStatuses on bt.TransactionStatusId equals ts.Id
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

            List<BalanceTransaction> transactions;
            try
            {
                transactions = await query.ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<BalanceTransaction>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, transactions, TimeSpan.FromSeconds(2));
            return RepositoryResult<IEnumerable<BalanceTransaction>>.Success(transactions);
        }
    }
}
