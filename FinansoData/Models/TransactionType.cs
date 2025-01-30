using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinansoData.Models
{
    /// <summary>
    /// TransactionType model
    /// </summary>
    public class TransactionType
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of transaction type
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // Navigation properties
        public virtual ICollection<BalanceTransaction> BalanceTransactions { get; set; }
        public virtual ICollection<TransactionCategory> TransactionsCategories { get; set; }
    }
}
