using System.ComponentModel.DataAnnotations;

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
