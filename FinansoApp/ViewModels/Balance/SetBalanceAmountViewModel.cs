using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace FinansoApp.ViewModels.Balance
{
    public class SetBalanceAmountViewModel
    {
        [Required]
        public int BalanceId { get; set; }

        public string? BalanceName { get; set; }
        public string? GroupName { get; set; }

        [Required]
        [Precision(18, 8)]
        public decimal Amount { get; set; }

        public bool IsCrypto { get; set; }

        public SetBalanceAmountViewModelErrorInfo Error { get; set; } = new SetBalanceAmountViewModelErrorInfo();

        public class SetBalanceAmountViewModelErrorInfo : Helpers.ErrorInfo
        {
            public bool InternalError { get; set; }
        }
    }
}
