using FinansoData.Data;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Transaction
{
    public class TransactionManagementRepository : ITransactionManagementRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public TransactionManagementRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<RepositoryResult<bool>> AddTransaction(decimal Amount, string? Description, int BalanceId, DateTime TransactionDate, string UserName, int TransactionTypeId, int TransactionStatusId)
        {
            // this value is used in db to easly get transactions
            var query = from balance in _applicationDbContext.Balances
                        where balance.Id == BalanceId
                        select new
                        {
                            groupId = balance.GroupId,
                            currencyId = balance.CurrencyId
                        };

            int groupId;
            int currencyId;
            string? userId;
            try
            {
                var data = await query.SingleOrDefaultAsync();
                groupId = data.groupId;
                currencyId = data.currencyId;

                userId = await _applicationDbContext.Users.Where(u => u.UserName == UserName).Select(u => u.Id).SingleOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure("Error while getting data from db", ErrorType.ServerError);
            }

            if(userId == null)
            {
                return RepositoryResult<bool>.Failure("No user found", ErrorType.NoUserFound);
            }


            BalanceTransaction balanceTransaction = new BalanceTransaction
            {
                Amount = Amount,
                Description = Description,
                BalanceId = BalanceId,
                TransactionDate = TransactionDate,
                AppUserId = userId,
                TransactionTypeId = TransactionTypeId,
                TransactionStatusId = TransactionStatusId,
                GroupId = groupId,
                CurrencyId = currencyId
            };

            try
            {
                _applicationDbContext.BalanceTransactions.Add(balanceTransaction);
                await _applicationDbContext.SaveChangesAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure("Error while adding transaction", ErrorType.ServerError);
            }

            return RepositoryResult<bool>.Success(true);
        }
    }
}
