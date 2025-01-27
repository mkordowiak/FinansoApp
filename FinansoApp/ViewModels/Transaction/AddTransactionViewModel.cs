﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinansoApp.ViewModels.Transaction
{
    public class AddTransactionViewModel
    {
        /// <summary>
        /// Balance id
        /// </summary>
        public int BalanceId { get; set; }

        /// <summary>
        /// Amount of transaction
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Description of transaction
        /// </summary>
        [Required(ErrorMessage = "Description is required")]
        [MinLength(3)]
        [StringLength(100, ErrorMessage = "Description can't be longer than 100 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Transaction date
        /// </summary>
        [Required(ErrorMessage = "Transaction date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        [Display(Name = "Transaction date")]
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Transaction type id
        /// </summary>
        [Required(ErrorMessage = "Transaction type is required")]
        [Display(Name = "Transaction type")]
        public int TransactionTypeId { get; set; }

        /// <summary>
        /// Transaction status id
        /// </summary>
        [Required(ErrorMessage = "Transaction status is required")]
        [Display(Name = "Transaction status")]
        public int TransactionStatusId { get; set; }

        /// <summary>
        /// Transaction types list for dropdown
        /// </summary>
        public IEnumerable<SelectListItem> TransactionTypes { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Transaction statuses list for dropdown
        /// </summary>
        public IEnumerable<SelectListItem> TransactionStatuses { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Balances list for dropdown
        /// </summary>
        public IEnumerable<SelectListItem> Balances { get; set; } = new List<SelectListItem>();


        /// <summary>
        /// Error info
        /// </summary>
        public AddTransactionErrorInfo Error { get; } = new AddTransactionErrorInfo();


        public class AddTransactionErrorInfo : Helpers.ErrorInfo
        {
            public bool GetDataInternalError { get; set; }
            public bool InternalError { get; set; } = false;
            public bool WrongData { get; set; } = false;
        }
    }
}
