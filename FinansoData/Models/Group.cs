using System.ComponentModel.DataAnnotations;

namespace FinansoData.Models
{
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

        /// <summary>
        /// Creation datetime
        /// </summary>
        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// Modification datetime
        /// </summary>
        public DateTime? Modified { get; set; }
    }
}
