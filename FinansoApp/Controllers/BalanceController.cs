using FinansoApp.ViewModels.Balance;
using FinansoData.DataViewModel.Balance;
using FinansoData.Repository.Balance;
using FinansoData.Repository.Currency;
using FinansoData.Repository.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Controllers
{
    public class BalanceController : Controller
    {
        private readonly IBalanceManagmentRepository _balanceManagmentRepository;
        private readonly ICurrencyQueryRepository _currencyRepository;
        private readonly IGroupQueryRepository _groupQueryRepository;
        private readonly IGroupUsersQueryRepository _groupUsersQueryRepository;
        private readonly IBalanceQueryRepository _balanceQueryRepository;

        public BalanceController(IBalanceManagmentRepository balanceManagmentRepository, ICurrencyQueryRepository currency, IGroupQueryRepository group, IGroupUsersQueryRepository groupUsersQueryRepository, IBalanceQueryRepository balanceQueryRepository)
        {
            _balanceManagmentRepository = balanceManagmentRepository;
            _currencyRepository = currency;
            _groupQueryRepository = group;
            _groupUsersQueryRepository = groupUsersQueryRepository;
            _balanceQueryRepository = balanceQueryRepository;
        }

        public async Task<IActionResult> Index()
        {
            FinansoData.RepositoryResult<IEnumerable<BalanceViewModel>> repositoryResult = await _balanceQueryRepository.GetListOfBalancesForUser(User.Identity.Name);

            if (!repositoryResult.IsSuccess)
            {
                return BadRequest();
            }

            return View(repositoryResult.Value);
        }


        #region AddBalance

        /// <summary>
        /// Add balance view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddBalance()
        {
            // Get all currencies and groups for current user
            FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel>> currencies = await _currencyRepository.GetAllCurrencies();
            FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>?> groups = await _groupQueryRepository.GetUserGroups(User.Identity.Name);

            if (currencies.IsSuccess == false || groups.IsSuccess == false)
            {
                return BadRequest();
            }

            if (currencies.Value == null || groups.Value == null
                || currencies.Value.Count() == 0 || groups.Value.Count() == 0)
            {
                return BadRequest();
            }

            AddBalanceViewModel addBalanceViewModel = new AddBalanceViewModel
            {
                Currencies = currencies.Value,
                Groups = groups.Value
            };

            return View(addBalanceViewModel);
        }

        /// <summary>
        /// Add balance post
        /// </summary>
        /// <param name="balanceVM"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddBalance(AddBalanceViewModel balanceVM)
        {
            if (!ModelState.IsValid)
            {
                // Get all currencies and groups for current user
                balanceVM.Currencies = (await _currencyRepository.GetAllCurrencies()).Value;
                balanceVM.Groups = (await _groupQueryRepository.GetUserGroups(User.Identity.Name)).Value;

                return View(balanceVM);
            }

            // Check if user has access to selected group
            FinansoData.RepositoryResult<FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel> groupMembershipResult = await _groupUsersQueryRepository.GetUserMembershipInGroupAsync(balanceVM.SelectedGroup, User.Identity.Name);
            if (!groupMembershipResult.IsSuccess)
            {
                return BadRequest();
            }

            if (!groupMembershipResult.Value.IsMember)
            {
                return Unauthorized();
            }


            // Get list of groups for current user and all currencies
            FinansoData.RepositoryResult<FinansoData.Models.Currency?> currency = await _currencyRepository.GetCurrencyModelById(balanceVM.SelectedCurrency);
            FinansoData.RepositoryResult<FinansoData.Models.Group?> group = await _groupQueryRepository.GetGroupById(balanceVM.SelectedGroup);

            if (currency.IsSuccess == false || group.IsSuccess == false)
            {
                return BadRequest();
            }

            if (currency.Value == null || group.Value == null)
            {
                return BadRequest();
            }

            BalanceViewModel newBalanceViewModel = new BalanceViewModel { Name = balanceVM.Name, Amount = 0, Currency = currency.Value, Group = group.Value };

            FinansoData.RepositoryResult<bool> repositoryResult = await _balanceManagmentRepository.AddBalance(newBalanceViewModel);

            if (repositoryResult.IsSuccess == false)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
