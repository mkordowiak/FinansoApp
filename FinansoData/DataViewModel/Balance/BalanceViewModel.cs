using FinansoData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.DataViewModel.Balance
{
    public class BalanceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public Models.Currency Currency { get; set; }
        public Models.Group Group { get; set; }
    }
}
