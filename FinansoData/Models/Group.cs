using System.ComponentModel.DataAnnotations;

namespace FinansoData.Models
{
    /// <summary>
    /// Group model
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Name of group
        /// </summary>
        [Required]
        public string Name { get; set; }

        [Required]
        public AppUser OwnerAppUser { get; set; }

        [Required]
        public ICollection<GroupUser> GroupUser { get; set; }

        [Required]
        public ICollection<Balance> Balance { get; set; }

        /// <summary>
        /// Creation datetime
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Modification datetime
        /// </summary>
        public DateTime? Modified { get; set; }

        // Navigation properties
        public virtual ICollection<BalanceTransaction> BalanceTransactions { get; set; }
    }
}
