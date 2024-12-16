using FinansoData.Data;
using FinansoData.DataViewModel.Currency;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Currency
{
    public class CurrencyQueryRepository : ICurrencyQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(60);

        public CurrencyQueryRepository(ApplicationDbContext context, FinansoData.Repository.ICacheWrapper cacheWrapper)
        {
            _context = context;
            _cacheWrapper = cacheWrapper;
        }

        public async Task<RepositoryResult<IEnumerable<CurrencyViewModel>>> GetAllCurrencies()
        {
            if (!_cacheWrapper.TryGetValue("AllCurrencies", out IEnumerable<CurrencyViewModel>? currencies))
            {
                IQueryable<CurrencyViewModel> query = from currency in _context.Currencies
                                                      select new CurrencyViewModel
                                                      {
                                                          Id = currency.Id,
                                                          Name = currency.Name
                                                      };

                try
                {
                    currencies = await query.ToListAsync();
                    _cacheWrapper.Set("AllCurrencies", currencies, _cacheDuration);
                    return RepositoryResult<IEnumerable<CurrencyViewModel>>.Success(currencies);
                }
                catch
                {
                    return RepositoryResult<IEnumerable<CurrencyViewModel>>.Failure(null, ErrorType.ServerError);
                }
            }
            return RepositoryResult<IEnumerable<CurrencyViewModel>>.Success(currencies);
        }

        public async Task<RepositoryResult<Models.Currency?>> GetCurrencyModelById(int id)
        {
            if (!_cacheWrapper.TryGetValue($"CurrencyModel_{id}", out Models.Currency? currency))
            {
                IQueryable<Models.Currency> query = from c in _context.Currencies
                                                    where c.Id == id
                                                    select c;

                try
                {
                    currency = await query.SingleOrDefaultAsync();
                    _cacheWrapper.Set($"CurrencyModel_{id}", currency, _cacheDuration);
                    return RepositoryResult<Models.Currency?>.Success(currency);
                }
                catch
                {
                    return RepositoryResult<Models.Currency>.Failure(null, ErrorType.ServerError);
                }
            }
            return RepositoryResult<Models.Currency?>.Success(currency);
        }

        public async Task<RepositoryResult<CurrencyViewModel?>> GetCurrencyById(int id)
        {
            if (!_cacheWrapper.TryGetValue($"Currency_{id}", out CurrencyViewModel? currency))
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
                    currency = await query.SingleOrDefaultAsync();
                    _cacheWrapper.Set($"Currency_{id}", currency, _cacheDuration);
                    return RepositoryResult<CurrencyViewModel>.Success(currency);
                }
                catch
                {
                    return RepositoryResult<CurrencyViewModel>.Failure(null, ErrorType.ServerError);
                }
            }
            return RepositoryResult<CurrencyViewModel>.Success(currency);
        }
    }
}
