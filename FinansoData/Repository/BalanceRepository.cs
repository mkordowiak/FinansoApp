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
    public class BalanceRepository : IBalanceRepository
    {
        private ApplicationDbContext _context;
        private readonly List<KeyValuePair<string, bool>> _errors;

        public BalanceRepository(ApplicationDbContext context)
        {
            _context = context;
            _errors = new List<KeyValuePair<string, bool>>();
        }

        public IEnumerable<KeyValuePair<string, bool>> Error
        {
            get => _errors;
        }


        #region CRUD operations
        public bool Add(Balance balance)
        {
            try
            {
                _context.Add(balance);
                return Save();
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }
        }

        public bool Delete(Balance balance)
        {
            try
            {
                _context.Remove(balance);
                return Save();
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                int saved = _context.SaveChanges();
                return saved > 0 ? true : false;

            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }

        }


        public async Task<bool> SaveAsync()
        {
            try
            {
                int saved = await _context.SaveChangesAsync();
                return saved > 0 ? true : false;
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }
        }

        public bool Update(Balance balance)
        {
            try
            {
                _context.Update(balance);
                return Save();
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Balance balance)
        {
            try
            {
                _context.Update(balance);
                return await SaveAsync();
            }
            catch
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return false;
            }
        }
        #endregion

        public async Task<IEnumerable<Balance>?> GetAllBalancesAsync()
        {
            try
            {
                return await _context.Balances.ToListAsync();
            }
            catch (Exception)
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return null;
            }
        }

        public async Task<Balance?> GetBalanceAsync(int id)
        {
            try
            {
                return await _context.Balances
                        .Include(a => a.Currency)
                        .FirstOrDefaultAsync(i => i.Id.Equals(id));
            }
            catch (Exception)
            {
                _errors.Add(new KeyValuePair<string, bool>("DatabaseError", true));
                return null;
            }
        }

        
    }
}
