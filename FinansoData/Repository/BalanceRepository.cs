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

        public BalanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Balance balance)
        {
            _context.Add(balance);
            return Save();
        }

        public bool Delete(Balance balance)
        {
            _context.Remove(balance);
            return Save();
        }

        public async Task<IEnumerable<Balance>> GetAllBalancesAsync()
        {
            return await _context.Balances.ToListAsync();
        }

        public async Task<Balance> GetBalanceAsync(int id)
        {
            return await _context.Balances
                .Include(a => a.Currency)
                .FirstOrDefaultAsync(i => i.Id.Equals(id));
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

        public bool Update(Balance balance)
        {
            _context.Update(balance);
            return Save();
        }
    }
}
