using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Required]
        public DateTime Created { get; set; } = DateTime.Now;
        
        public DateTime? Modified { get; set; }
    }
}
