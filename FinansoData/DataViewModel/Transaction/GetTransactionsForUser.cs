namespace FinansoData.DataViewModel.Transaction
{
    public class GetTransactionsForUser
    {
        public int TransactionId { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int BalanceId { get; set; }
        public string BalanceName { get; set; }
        public string Description { get; set; }
        public string TransactionType { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
