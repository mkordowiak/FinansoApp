using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinansoData.Models
{
    /// <summary>
    /// GroupUser model
    /// </summary>
    public class GroupUser
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; }

        [Required]
        public string AppUserId { get; set; }

        /// <summary>
        /// AppUser foreign key
        /// </summary>
        [ForeignKey(nameof(AppUserId))]
        public AppUser AppUser { get; set; }

        /// <summary>
        /// Is user active
        /// </summary>
        [Required]
        public bool Active { get; set; } = true;

        /// <summary>
        /// Creation datetime
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Modification datetime
        /// </summary>
        public DateTime? Updated { get; set; }
    }
}
