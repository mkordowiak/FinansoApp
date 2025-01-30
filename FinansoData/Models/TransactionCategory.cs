using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Models
{
    public class TransactionCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int TransactionTypeId { get; set; }

        // Navigation properties
        public virtual ICollection<BalanceTransaction> BalanceTransactions { get; set; }
    }
}
