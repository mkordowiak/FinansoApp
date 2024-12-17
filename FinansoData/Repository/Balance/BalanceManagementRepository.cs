using FinansoData.Data;
using FinansoData.DataViewModel.Balance;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Balance
{
    public class BalanceManagementRepository : IBalanceManagmentRepository
    {
        private readonly ApplicationDbContext _context;

        public BalanceManagementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RepositoryResult<bool>> AddBalance(BalanceViewModel balance)
        {
            Models.Balance newBalance = new Models.Balance
            {
                Name = balance.Name,
                Amount = balance.Amount,
                CurrencyId = balance.Currency.Id,
                GroupId = balance.Group.Id
            };

            try
            {
                _context.Balances.Add(newBalance);
                await _context.SaveChangesAsync();

                return RepositoryResult<bool>.Success(true);
            }
            catch
            {
                return RepositoryResult<bool>.Failure(null, ErrorType.ServerError);
            }
        }

        public Task<RepositoryResult<bool>> DeleteBalance(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<RepositoryResult<bool?>> SetBalanceAmount(int id, double amount)
        {
            Models.Balance? balance;

            try
            {
                balance = await _context.Balances.SingleOrDefaultAsync(x => x.Id == id);
            }
            catch
            {
                return RepositoryResult<bool?>.Failure("Can't access database", ErrorType.ServerError);
            }

            if(balance == null)
            {
                return RepositoryResult<bool?>.Failure("Can't find balance", ErrorType.NotFound);
            }

            try
            {

                balance.Amount = amount;
                balance.Modified = DateTime.Now;
                await _context.SaveChangesAsync();
                return RepositoryResult<bool?>.Success(true);

            }
            catch
            {
                return RepositoryResult<bool?>.Failure("Can't update balance", ErrorType.ServerError);
            }
            
        }

        public Task<RepositoryResult<bool>> UpdateBalance(BalanceViewModel balance)
        {
            throw new NotImplementedException();
        }
    }
}
