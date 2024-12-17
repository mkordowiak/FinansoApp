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
        [Range(0, double.MaxValue)]
        public double Amount { get; set; }

        public SetBalanceAmountViewModelErrorInfo Error { get; set; } = new SetBalanceAmountViewModelErrorInfo();

        public class SetBalanceAmountViewModelErrorInfo : Helpers.ErrorInfo
        {
            public bool InternalError { get; set; }
        }
    }
}
