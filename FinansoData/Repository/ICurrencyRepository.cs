using FinansoData.Models;

namespace FinansoData.Repository
{
    public interface ICurrencyRepository
    {
        ICurrencyRepositoryErrorInfo Error { get; }

        public interface ICurrencyRepositoryErrorInfo
        {
            bool DatabaseError { get; set; }
        }


        Task<Currency?> GetCurrencyAsync(int currencyId);
        Task<IEnumerable<Currency>?> GetAllCurrencyAsync();

        bool Add(Currency currency);
        bool Update(Currency currency);
        bool Delete(Currency currency);
        Task<bool> UpdateAsync(Currency currency);
        bool Save();
        Task<bool> SaveAsync();
    }

    public class CurrencyRepositoryErrorInfo : ICurrencyRepository.ICurrencyRepositoryErrorInfo
    {
        public bool DatabaseError { get; set; }
    }
}
