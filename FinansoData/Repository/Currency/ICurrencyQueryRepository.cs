using FinansoData.DataViewModel.Currency;

namespace FinansoData.Repository.Currency
{
    public interface ICurrencyQueryRepository
    {
        /// <summary>
        /// Get all currencies
        /// </summary>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<CurrencyViewModel>>> GetAllCurrencies();

        /// <summary>
        /// Get currency by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RepositoryResult<CurrencyViewModel?>> GetCurrencyById(int id);

        Task<RepositoryResult<Models.Currency?>> GetCurrencyModelById(int id);
    }
}
