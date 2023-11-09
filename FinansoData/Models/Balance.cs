using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Models
{
    public class Balance
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Balance name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Amount of fund on balance
        /// </summary>
        public Double Amount { get; set; }

        /// <summary>
        /// foreign key
        /// </summary>
        [Required]
        public Currency Currency { get; set; }

        /// <summary>
        /// Group
        /// </summary>
        public Group Group { get; set; }
    }
}
