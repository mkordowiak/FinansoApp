using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Models
{
    public class GroupUser
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

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
