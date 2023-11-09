using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Models
{
    public class TransactionType
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Is transaction outgoing
        /// </summary>
        [Required]
        public bool Outgoing { get; set; } = false;

        /// <summary>
        /// Is transaction incoming
        /// </summary>
        [Required]
        public bool Incoming { get; set; } = false;
    }
}
