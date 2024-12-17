using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinansoData.Models
{
    /// <summary>
    /// Application user model
    /// </summary>
    public class AppUser : IdentityUser
    {
        /// <summary>
        /// First name of user
        /// </summary>
        [MaxLength(100)]
        [MinLength(2)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Lase name of user
        /// </summary>
        [MaxLength(100)]
        [MinLength(2)]
        public string? LastName { get; set; }

        /// <summary>
        /// Nickname of user
        /// </summary>
        public string? Nickname { get; set; }

        public string? Image { get; set; }

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
