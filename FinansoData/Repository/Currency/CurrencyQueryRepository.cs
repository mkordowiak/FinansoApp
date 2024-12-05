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
            try
            {
                RepositoryResult<Models.Currency>? result = await _context.Currencies.Where(x => x.Id == id)
                    .Select(x => RepositoryResult<Models.Currency>.Success(x))
                    .SingleOrDefaultAsync();

                return result;
            }
            catch
            {
                return RepositoryResult<Models.Currency>.Failure(null, ErrorType.ServerError);
            }
        }

        public async Task<RepositoryResult<CurrencyViewModel>> GetCurrencyById(int id)
        {
            try
            {
                RepositoryResult<CurrencyViewModel>? result = await _context.Currencies.Where(x => x.Id == id)
                    .Select(x => new CurrencyViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .Select(x => RepositoryResult<CurrencyViewModel>.Success(x))
                    .SingleOrDefaultAsync();

                return result;
            }
            catch
            {
                return RepositoryResult<CurrencyViewModel>.Failure(null, ErrorType.ServerError);
            }
        }
    }
}
