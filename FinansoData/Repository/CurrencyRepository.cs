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
        private readonly List<KeyValuePair<string, bool>> _errors;

        public CurrencyRepository(ApplicationDbContext context)
        {
            _context = context;

            _errors = new List<KeyValuePair<string, bool>>();
        }

        public IEnumerable<KeyValuePair<string, bool>> Error
        {
            get => _errors;
        }

        #region CRUD operations
        public bool Add(Currency currency)
        {
            try
            {
                _context.Add(currency);
                return Save();
            }
            catch
            {

                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }
            
        }

        public bool Delete(Currency currency)
        {
            try
            {
                _context.Remove(currency);
                return Save();
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }
        }

        public bool Update(Currency currency)
        {
            try
            {
                _context.Update(currency);
                return Save();
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Currency currency)
        {
            try
            {
                _context.Update(currency);
                return await SaveAsync();
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }
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
        #endregion

        public async Task<IEnumerable<Currency>?> GetAllCurrencyAsync()
        {
            try
            {
                return await _context.Currencies.ToListAsync();
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return null;
            }
        }

        public async Task<Currency?> GetCurrencyAsync(int currencyId)
        {
            try
            {
                return await _context.Currencies.FirstAsync(c => c.Id == currencyId);
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return null;
            }
        }

        

        
    }
}
