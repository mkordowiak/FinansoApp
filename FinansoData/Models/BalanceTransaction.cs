﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinansoData.Models
{
    /// <summary>
    /// Balance transaction model
    /// </summary>
    public class BalanceTransaction
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        [Required]
        [Precision(18, 8)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [MaxLength(256)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        [Required]
        public string AppUserId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int BalanceId { get; set; }

        [Required]
        public int TransactionTypeId { get; set; }

        [Required]
        public int TransactionStatusId { get; set; }

        [Required]
        public int TransactionCategoryId { get; set; }

        [MaxLength(64)]
        public string? RecurringTransactionId { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        // Navigation properties
        public virtual AppUser AppUser { get; set; }
        public virtual Group Group { get; set; }
        public virtual Balance Balance { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        public virtual TransactionStatus TransactionStatus { get; set; }
        public virtual TransactionCategory TransactionCategory { get; set; }
        public virtual Currency Currency { get; set; }

    }
}
