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
        private readonly IBalanceManagementRepository _balanceManagmentRepository;
        private readonly ICurrencyQueryRepository _currencyRepository;
        private readonly IGroupQueryRepository _groupQueryRepository;
        private readonly IGroupUsersQueryRepository _groupUsersQueryRepository;
        private readonly IBalanceQueryRepository _balanceQueryRepository;
        private readonly IBalanceSumAmount _balanceSumAmount;
        

        public BalanceController(
            IBalanceManagementRepository balanceManagmentRepository, ICurrencyQueryRepository currency, IGroupQueryRepository group, IGroupUsersQueryRepository groupUsersQueryRepository, IBalanceQueryRepository balanceQueryRepository, IBalanceSumAmount balanceSumAmount)
        {
            _balanceManagmentRepository = balanceManagmentRepository;
            _currencyRepository = currency;
            _groupQueryRepository = group;
            _groupUsersQueryRepository = groupUsersQueryRepository;
            _balanceQueryRepository = balanceQueryRepository;
            _balanceSumAmount = balanceSumAmount;
            _balanceSumAmount = balanceSumAmount;
        }

        public async Task<IActionResult> Index()
        {

            // Get list of balances for user
            FinansoData.RepositoryResult<IEnumerable<BalanceViewModel>> repositoryResult = await _balanceQueryRepository.GetListOfBalancesForUser(User.Identity.Name);
            if (!repositoryResult.IsSuccess)
            {
                return BadRequest();
            }

            // Get sum of all balances for user
            FinansoData.RepositoryResult<decimal?> sumAmountResult = await _balanceSumAmount.GetBalancesSumAmountForUser(User.Identity.Name);
            if (!sumAmountResult.IsSuccess)
            {
                BadRequest();
            }

            ViewModels.Balance.IndexViewModel indexViewModel = new ViewModels.Balance.IndexViewModel();
            indexViewModel.Balances = repositoryResult.Value;
            indexViewModel.SumAmount = sumAmountResult.Value;

            return View(indexViewModel);
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

        #region SetBalanceAmount


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SetBalanceAmount(int id)
        {
            // Check if user has access to selected group
            FinansoData.RepositoryResult<bool?> hasUserAccessToGroup = await _balanceQueryRepository.HasUserAccessToBalance(User.Identity.Name, id);

            if (!hasUserAccessToGroup.IsSuccess)
            {
                return BadRequest();
            }

            if (hasUserAccessToGroup.Value == false)
            {
                return Unauthorized();
            }

            var balance = await _balanceQueryRepository.GetBalance(id);

            SetBalanceAmountViewModel setBalanceAmountViewModel = new SetBalanceAmountViewModel
            {
                BalanceName = balance.Value.Name,
                GroupName = balance.Value.Group.Name,
                BalanceId = id,
                Amount = balance.Value.Amount,
                IsCrypto = balance.Value.Currency.Code == "BTC" || balance.Value.Currency.Code == "ETH" ? true : false
            };
            return View(setBalanceAmountViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetBalanceAmount(SetBalanceAmountViewModel setBalanceAmountViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(setBalanceAmountViewModel);
            }

            // Check if user has access to selected group
            FinansoData.RepositoryResult<bool?> hasUserAccessToGroup = await _balanceQueryRepository.HasUserAccessToBalance(User.Identity.Name, setBalanceAmountViewModel.BalanceId);

            if (!hasUserAccessToGroup.IsSuccess)
            {
                return BadRequest();
            }

            if (hasUserAccessToGroup.Value == false)
            {
                return Unauthorized();
            }


            FinansoData.RepositoryResult<bool?> repositoryResult = await _balanceManagmentRepository.SetBalanceAmount(setBalanceAmountViewModel.BalanceId, setBalanceAmountViewModel.Amount);
            if (repositoryResult.IsSuccess == false)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Balance");
        }

        #endregion
    }
}
