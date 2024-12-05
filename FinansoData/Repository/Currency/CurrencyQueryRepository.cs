using FinansoData.Data;
using FinansoData.DataViewModel.Currency;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Currency
{
    public class CurrencyQueryRepository : ICurrencyQueryRepository
    {
        private readonly ApplicationDbContext _context;
        public CurrencyQueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RepositoryResult<IEnumerable<CurrencyViewModel>>> GetAllCurrencies()
        {
            IQueryable<CurrencyViewModel> query = from currency in _context.Currencies
                                                  select new CurrencyViewModel
                                                  {
                                                      Id = currency.Id,
                                                      Name = currency.Name
                                                  };

            try
            {
                return RepositoryResult<IEnumerable<CurrencyViewModel>>.Success(await query.ToListAsync());
            }
            catch
            {
                return RepositoryResult<IEnumerable<CurrencyViewModel>>.Failure(null, ErrorType.ServerError);
            }
        }

        public async Task<RepositoryResult<Models.Currency?>> GetCurrencyModelById(int id)
        {
            var query = from currency in _context.Currencies
                        where currency.Id == id
                        select currency;

            try
            {
                var resutl = await query.SingleOrDefaultAsync();

                return RepositoryResult<Models.Currency?>.Success(resutl);
            }
            catch
            {
                return RepositoryResult<Models.Currency>.Failure(null, ErrorType.ServerError);
            }
        }

        public async Task<RepositoryResult<CurrencyViewModel?>> GetCurrencyById(int id)
        {
            IQueryable<CurrencyViewModel> query = from c in _context.Currencies
                                                  where c.Id == id
                                                  select new CurrencyViewModel
                                                  {
                                                      Id = c.Id,
                                                      Name = c.Name
                                                  };

            try
            {
                CurrencyViewModel? result = await query.SingleOrDefaultAsync();
                return RepositoryResult<CurrencyViewModel>.Success(result);
            }
            catch
            {
                return RepositoryResult<CurrencyViewModel>.Failure(null, ErrorType.ServerError);
            }
        }
    }
}
