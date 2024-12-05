using FinansoData.Data;
using FinansoData.DataViewModel.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var newBalance = new Models.Balance
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

        public Task<RepositoryResult<bool>> UpdateBalance(BalanceViewModel balance)
        {
            throw new NotImplementedException();
        }
    }
}
