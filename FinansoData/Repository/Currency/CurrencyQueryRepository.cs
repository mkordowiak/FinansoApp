using FinansoData.Data;
using FinansoData.DataViewModel.Currency;
using FinansoData.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Currency
{
    public class CurrencyQueryRepository : ICurrencyQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(60);
        private readonly string _cacheClassName;

        public CurrencyQueryRepository(ApplicationDbContext context, FinansoData.Repository.ICacheWrapper cacheWrapper)
        {
            _context = context;
            _cacheWrapper = cacheWrapper;
            _cacheClassName = this.GetType().Name;
        }

        public async Task<RepositoryResult<IEnumerable<CurrencyViewModel>>> GetAllCurrencies()
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<CurrencyViewModel>? cacheAllCurrencies))
            {
                return RepositoryResult<IEnumerable<CurrencyViewModel>>.Success(cacheAllCurrencies);
            }

            IQueryable<CurrencyViewModel> query = from currency in _context.Currencies.AsNoTracking()
                                                  select new CurrencyViewModel
                                                  {
                                                      Id = currency.Id,
                                                      Name = currency.Name
                                                  };


            IEnumerable<CurrencyViewModel>? currencies;
            try
            {
                currencies = await query.ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<CurrencyViewModel>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, currencies, _cacheDuration);
            return RepositoryResult<IEnumerable<CurrencyViewModel>>.Success(currencies);
        }

        public async Task<RepositoryResult<Models.Currency?>> GetCurrencyModelById(int id)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{id}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out Models.Currency? cacheCurrency))
            {
                return RepositoryResult<Models.Currency?>.Success(cacheCurrency);
            }

            IQueryable<Models.Currency> query = from c in _context.Currencies
                                                where c.Id == id
                                                select c;

            Models.Currency? currency;
            try
            {
                currency = await query.AsNoTracking().SingleOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<Models.Currency>.Failure(null, ErrorType.ServerError);
            }

            if (currency == null)
            {
                return RepositoryResult<Models.Currency>.Success(null);
            }

            _cacheWrapper.Set(cacheDataKey, currency, _cacheDuration);
            return RepositoryResult<Models.Currency?>.Success(currency);
        }

        public async Task<RepositoryResult<CurrencyViewModel?>> GetCurrencyById(int id)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{id}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out CurrencyViewModel? cacheCurrency))
            {
                return RepositoryResult<CurrencyViewModel?>.Success(cacheCurrency);
            }

            IQueryable<CurrencyViewModel> query = from c in _context.Currencies.AsNoTracking()
                                                  where c.Id == id
                                                  select new CurrencyViewModel
                                                  {
                                                      Id = c.Id,
                                                      Name = c.Name
                                                  };

            CurrencyViewModel? currency;
            try
            {
                currency = await query.SingleOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<CurrencyViewModel>.Failure(null, ErrorType.ServerError);
            }

            if (currency == null)
            {
                return RepositoryResult<CurrencyViewModel>.Success(null);
            }

            _cacheWrapper.Set(cacheDataKey, currency, _cacheDuration);
            return RepositoryResult<CurrencyViewModel?>.Success(currency);
        }
    }
}
