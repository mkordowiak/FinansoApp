using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.DataViewModel.Transaction
{
    public class GetTransactionsForBalance
    {
        public int TransactionId { get; set; }
        public string? Description { get; set; }
        public string TransactionType { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
    }

}
