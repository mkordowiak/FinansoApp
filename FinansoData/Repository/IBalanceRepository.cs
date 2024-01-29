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
        /// <summary>
        /// Errors while performing operations
        /// </summary>
        IEnumerable<KeyValuePair<string, bool>> Error { get; }


        bool Add(Balance balance);
        bool Update(Balance balance);
        bool Delete(Balance balance);
        bool Save();
        Task<bool> SaveAsync();
        Task<bool> UpdateAsync(Balance balance);



        Task<Balance?> GetBalanceAsync(int id);
        Task<IEnumerable<Balance>?> GetAllBalancesAsync();
        

    }
}
