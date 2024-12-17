using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Models
{
    /// <summary>
    /// TransactionStatus model
    /// </summary>
    public class TransactionStatus
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of status
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // Navigation properties
        public virtual ICollection<BalanceTransaction> BalanceTransactions { get; set; }
    }
}
