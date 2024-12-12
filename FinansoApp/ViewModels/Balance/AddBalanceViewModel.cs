using FinansoData.DataViewModel.Currency;
using System.ComponentModel.DataAnnotations;

namespace FinansoApp.ViewModels.Balance
{
    public class AddBalanceViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(64)]
        [MinLength(3)]
        public string Name { get; set; }
        public IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel>? Currencies { get; set; }
        public int SelectedCurrency { get; set; }
        public IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>? Groups { get; set; }
        public int SelectedGroup { get; set; }
        public AddBalanceErrorInfo Error { get; set; } = new AddBalanceErrorInfo();

        public class AddBalanceErrorInfo : Helpers.ErrorInfo
        {
            public bool InternalError { get; set; }
        }
    }
}
