using System.ComponentModel.DataAnnotations;

namespace FinansoData.Models
{
    public class BalanceTransaction
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public Currency Currency { get; set; }

        public Balance Balance { get; set; }

        public TransactionType TransactionType { get; set; }

        public int Day { get; set; }

        public DateTime Date { get; set; }

    }
}
