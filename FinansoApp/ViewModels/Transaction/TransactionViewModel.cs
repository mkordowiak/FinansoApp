namespace FinansoApp.ViewModels.Transaction
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public string BalanceName { get; set; }
        public string GroupName { get; set; }
        public int TransactionId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionType { get; set; }

    }
}
