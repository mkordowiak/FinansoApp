using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinansoData.Models
{
    public class BalanceLog
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Amount of fund on balance
        /// </summary>
        [Required]
        public Decimal Amount { get; set; }

        /// <summary>
        /// Date of log
        /// </summary>
        [Required]
        [DefaultValue("GETDATE()")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Balance foreign key
        /// </summary>
        [Required]
        public int BalanceId { get; set; }

        [ForeignKey(nameof(BalanceId))]
        public Balance Balance { get; set; }
    }
}
