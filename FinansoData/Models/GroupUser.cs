using System.ComponentModel.DataAnnotations;

namespace FinansoData.Models
{
    public class GroupUser
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        public Group Group { get; set; }

        /// <summary>
        /// AppUser foreign key
        /// </summary>
        [Required]
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
        public DateTime Created { get; set; }

        /// <summary>
        /// Modification datetime
        /// </summary>
        public DateTime? Updated { get; set; }
    }
}
