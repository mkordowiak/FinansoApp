using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinansoData.Models;

namespace FinansoData.Repository
{
    public interface ICurrencyRepository
    {
        /// <summary>
        /// Errors while performing operations
        /// </summary>
        IEnumerable<KeyValuePair<string, bool>> Error { get; }


        Task<Currency?> GetCurrencyAsync(int currencyId);
        Task<IEnumerable<Currency>?> GetAllCurrencyAsync();

        bool Add (Currency currency);
        bool Update (Currency currency);
        bool Delete (Currency currency);
        Task<bool> UpdateAsync(Currency currency);
        bool Save();
        Task<bool> SaveAsync();
    }
}
