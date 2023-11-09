using FinansoData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository
{
    public interface IBalanceRepository
    {
        Task<Balance> GetBalanceAsync(int id);
        Task<IEnumerable<Balance>> GetAllBalancesAsync();
        bool Add(Balance balance);
        bool Update(Balance balance);
        bool Delete(Balance balance);
        bool Save();
        Task<bool> SaveAsync();

    }
}
