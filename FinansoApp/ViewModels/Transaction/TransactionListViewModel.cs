namespace FinansoApp.ViewModels.Transaction
{
    public class TransactionListViewModel
    {
        public List<TransactionViewModel> Transactions { get; set; }
        public int CurrentPage { get; set; }
        public int PagesCount { get; set; }
    }
}
