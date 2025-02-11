using FinansoData.Data;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Procedures
{
    public class SumBalanceProcedure : ISumBalanceProcedure
    {
        private readonly ApplicationDbContext _context;

        public SumBalanceProcedure(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<RepositoryResult<bool>> UpdateBalancesAndTransactions(int NumberOfRowsToBeProceeded)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync($"EXEC [UpdateBalanceTransactions] @NumOfRowsToProcedure = {NumberOfRowsToBeProceeded};");
            }
            catch (Exception e)
            {
                return RepositoryResult<bool>.Failure(e.Message, ErrorType.ServerError);
            }

            return RepositoryResult<bool>.Success(true);
        }
    }
}
