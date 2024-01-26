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
        Task<Currency> GetCurrencyAsync(int currencyId);
        Task<IEnumerable<Currency>> GetAllCurrencyAsync();

        bool Add (Currency currency);
        bool UpdaterThan (Currency currency);
        bool Delete (Currency currency);
        bool Save();
        Task<bool> SaveAsync();
    }
}
