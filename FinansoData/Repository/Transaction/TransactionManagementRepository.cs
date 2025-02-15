using FinansoData.Data;
using FinansoData.Enum;
using FinansoData.Models;
using FinansoData.Repository.Balance;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Transaction
{
    public class TransactionManagementRepository : ITransactionManagementRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IBalanceManagementRepository _balanceManagementRepository;

        public TransactionManagementRepository(ApplicationDbContext applicationDbContext, IBalanceManagementRepository balanceManagementRepository)
        {
            _applicationDbContext = applicationDbContext;
            _balanceManagementRepository = balanceManagementRepository;
        }

        private enum RecurringTransactionTypes
        {
            Monthly,
            Weekly
        }

        private List<BalanceTransaction> CreateRecuringTransactionsList(RecurringTransactionTypes recurringTransactionType, decimal Amount, string? Description, int BalanceId, DateTime TransactionStartDate, DateTime TransactionEndDate, string UserId, int TransactionTypeId, int TransactionCategoryId, int GroupId, int CurrencyId)
        {
            List<BalanceTransaction> balanceTransactions = new List<BalanceTransaction>();
            string recurringTransactionId = Guid.NewGuid().ToString();
            DateTime iterationDate = TransactionStartDate;

            while (iterationDate <= TransactionEndDate)
            {
                BalanceTransaction newBalanceTransaction = new BalanceTransaction
                {
                    Amount = Amount,
                    Description = Description,
                    BalanceId = BalanceId,
                    TransactionDate = iterationDate,
                    AppUserId = UserId,
                    TransactionTypeId = TransactionTypeId,
                    TransactionStatusId = (int)TransactionStatuses.Planned,
                    GroupId = GroupId,
                    CurrencyId = CurrencyId,
                    TransactionCategoryId = TransactionCategoryId,
                    RecurringTransactionId = recurringTransactionId
                };
                balanceTransactions.Add(newBalanceTransaction);
                if (recurringTransactionType == RecurringTransactionTypes.Monthly)
                {
                    iterationDate = iterationDate.AddMonths(1);
                }
                else if (recurringTransactionType == RecurringTransactionTypes.Weekly)
                {
                    iterationDate = iterationDate.AddDays(7);
                }
            }

            return balanceTransactions;
        }

        private async Task<RepositoryResult<bool>> AddTransactionsRecuring(RecurringTransactionTypes recurringTransactionTypes, decimal Amount, string? Description, int BalanceId, DateTime TransactionStartDate, DateTime TransactionEndDate, string UserName, int TransactionTypeId, int TransactionCategoryId)
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

                if (data == null)
                {
                    return RepositoryResult<bool>.Failure("No balance found", ErrorType.NotFound);
                }

                groupId = data.groupId;
                currencyId = data.currencyId;

                userId = await _applicationDbContext.Users.Where(u => u.UserName == UserName).Select(u => u.Id).SingleOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure("Error while getting data from db", ErrorType.ServerError);
            }

            if (userId == null)
            {
                return RepositoryResult<bool>.Failure("No user found", ErrorType.NoUserFound);
            }

            List<BalanceTransaction> balanceTransactions = CreateRecuringTransactionsList(recurringTransactionTypes, Amount, Description, BalanceId, TransactionStartDate, TransactionEndDate, userId, TransactionTypeId, TransactionCategoryId, groupId, currencyId);


            try
            {
                await _applicationDbContext.BalanceTransactions.AddRangeAsync(balanceTransactions);
                await _applicationDbContext.SaveChangesAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure("Error while adding transaction", ErrorType.ServerError);
            }

            return RepositoryResult<bool>.Success(true);
        }

        public async Task<RepositoryResult<bool>> AddTransaction(decimal Amount, string? Description, int BalanceId, DateTime TransactionDate, string UserName, int TransactionTypeId, int TransactionStatusId, int TransactionCategoryId)
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

                if (data == null)
                {
                    return RepositoryResult<bool>.Failure("No balance found", ErrorType.NotFound);
                }

                groupId = data.groupId;
                currencyId = data.currencyId;

                userId = await _applicationDbContext.Users.Where(u => u.UserName == UserName).Select(u => u.Id).SingleOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure("Error while getting data from db", ErrorType.ServerError);
            }

            if (userId == null)
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
                CurrencyId = currencyId,
                TransactionCategoryId = TransactionCategoryId
            };

            try
            {
                await _applicationDbContext.BalanceTransactions.AddAsync(balanceTransaction);
                await _applicationDbContext.SaveChangesAsync();
            }
            catch
            {
                return RepositoryResult<bool>.Failure("Error while adding transaction", ErrorType.ServerError);
            }


            // if transaction is completed, update balance
            if (balanceTransaction.TransactionStatusId == (int)TransactionStatuses.Completed)
            {
                decimal amountToAdd = (balanceTransaction.TransactionTypeId == (int)FinansoData.Enum.TransactionTypes.Income) ? Amount : -Amount;
                return await _balanceManagementRepository.AddToBalanceAmount(BalanceId, amountToAdd);
            }


            return RepositoryResult<bool>.Success(true);
        }



        public async Task<RepositoryResult<bool>> AddTransactionMonthlyRecurring(decimal Amount, string? Description, int BalanceId, DateTime TransactionStartDate, DateTime TransactionEndDate, string UserName, int TransactionTypeId, int TransactionCategoryId)
        {
            return await AddTransactionsRecuring(RecurringTransactionTypes.Monthly, Amount, Description, BalanceId, TransactionStartDate, TransactionEndDate, UserName, TransactionTypeId, TransactionCategoryId);
        }


        public async Task<RepositoryResult<bool>> AddTransactionWeeklyRecurring(decimal Amount, string? Description, int BalanceId, DateTime TransactionStartDate, DateTime TransactionEndDate, string UserName, int TransactionTypeId, int TransactionCategoryId)
        {
            return await AddTransactionsRecuring(RecurringTransactionTypes.Weekly, Amount, Description, BalanceId, TransactionStartDate, TransactionEndDate, UserName, TransactionTypeId, TransactionCategoryId);
        }
    }
}
