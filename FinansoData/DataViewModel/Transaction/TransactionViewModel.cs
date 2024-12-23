using FinansoData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.DataViewModel.Transaction
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public AppUser User { get; set; }
        public Models.Group? Group { get; set; }
        public Models.Balance Balance { get; set; }
        public TransactionType transactionType { get; set; }
        public TransactionStatus transactionStatus { get; set; }
        public Models.Currency Currency { get; set; }
    }
}
