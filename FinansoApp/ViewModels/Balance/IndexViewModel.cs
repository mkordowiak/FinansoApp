using FinansoData.DataViewModel.Balance;

namespace FinansoApp.ViewModels.Balance
{
    public class IndexViewModel
    {
        public double? SumAmount { get; set; }
        public IEnumerable<BalanceViewModel> Balances { get; set; }

        public IndexErrorInfo Error { get; set; } = new IndexErrorInfo();

        public class IndexErrorInfo : Helpers.ErrorInfo
        {
            public bool InternalError { get; set; }
        }
    }
}
