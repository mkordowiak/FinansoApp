using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace FinansoData.Models
{
    /// <summary>
    /// Balance model
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Balance name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Amount of fund on balance
        /// </summary>
        [Required]
        public Double Amount { get; set; }

        /// <summary>
        /// Currency foreign key
        /// </summary>
        [Required]
        public int CurrencyId { get; set; }

        /// <summary>
        /// foreign key
        /// </summary>
        [ForeignKey(nameof(CurrencyId))]
        public Currency Currency { get; set; }

        /// <summary>
        /// Group foreign key
        /// </summary>
        [Required]
        public int GroupId { get; set; }

        /// <summary>
        /// Group
        /// </summary>
        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; }

        /// <summary>
        /// Creation datetime
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Modification datetime
        /// </summary>
        public DateTime? Modified { get; set; }


        // Navigation properties
        public virtual ICollection<BalanceTransaction> BalanceTransactions { get; set; }

    }
}
