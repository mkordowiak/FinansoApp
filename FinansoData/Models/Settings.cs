using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Models
{
    /// <summary>
    /// Settings model
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Key (name) of setting
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Key { get; set; }

        /// <summary>
        /// Value of setting
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Type of setting
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Type { get; set; } = "string";

        /// <summary>
        /// Description of setting
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// DateTime of setting update
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
