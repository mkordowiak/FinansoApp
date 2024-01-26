using FinansoData.Data;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private ApplicationDbContext _context;

        public CurrencyRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public bool Add(Currency currency)
        {
            _context.Add(currency);
            return Save();
        }

        public bool Delete(Currency currency)
        {
            _context.Remove(currency);
            return Save();
        }

        public async Task<IEnumerable<Currency>> GetAllCurrencyAsync()
        {
            return await _context.Currencies.ToListAsync();
        }

        public async Task<Currency> GetCurrencyAsync(int currencyId)
        {
            return await _context.Currencies.FirstAsync(c => c.Id == currencyId);
        }

        public bool Save()
        {
            int saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public async Task<bool> SaveAsync()
        {
            int saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public bool UpdaterThan(Currency currency)
        {
            _context.Update(currency);
            return Save();
        }
    }
}
