using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Models
{
    public class Currency
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of currency
        /// </summary>
        [Required]
        [MaxLength(6)]
        public string Name { get; set; }

        /// <summary>
        /// Currency exchange rate
        /// </summary>
        [Required]
        public Double ExchangeRate { get; set; } = 1;

        /// <summary>
        /// DateTime of ExchangeRate field update
        /// </summary>
        [Required]
        public DateTime Updated { get; set; }
    }
}
