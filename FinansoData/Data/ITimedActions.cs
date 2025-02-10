using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Data
{
    public interface ITimedActions
    {
        /// <summary>
        /// Timed action to update balance transactions
        /// </summary>
        /// <returns></returns>
        Task UpdateBalanceTransactions();
    }
}
