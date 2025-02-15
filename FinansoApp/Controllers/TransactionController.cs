using AutoMapper;
using FinansoApp.ViewModels.Transaction;
using FinansoData.Repository.Balance;
using FinansoData.Repository.Settings;
using FinansoData.Repository.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinansoApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionsQueryRepository _transactionQueryRepository;
        private readonly ITransactionManagementRepository _transactionManagementRepository;
        private readonly ITransactionMetaQueryRepository _transactionMetaQueryRepository;
        private readonly IBalanceQueryRepository _balanceQueryRepository;
        private readonly IMapper _mapper;
        private readonly ISettingsQueryRepository _settingsQueryRepository;

        public TransactionController(
            ITransactionsQueryRepository transactionQueryRepository,
            ITransactionManagementRepository transactionManagementRepository,
            ITransactionMetaQueryRepository transactionMetaQueryRepository,
            IBalanceQueryRepository balanceQueryRepository,
            IMapper mapper,
            ISettingsQueryRepository settingsQueryRepository)
        {
            _transactionQueryRepository = transactionQueryRepository;
            _transactionManagementRepository = transactionManagementRepository;
            _transactionMetaQueryRepository = transactionMetaQueryRepository;
            _balanceQueryRepository = balanceQueryRepository;
            _mapper = mapper;
            _settingsQueryRepository = settingsQueryRepository;
        }

        #region private methods

        private async Task<AddTransactionViewModel> GetDataForAddTransaction(int? balanceId, string userName)
        {
            AddTransactionViewModel addTransactionViewModel = new AddTransactionViewModel();

            FinansoData.RepositoryResult<IEnumerable<Tuple<int, string>>> transactionStatuses = await _transactionMetaQueryRepository.GetShortListOfAllTransactionStatuses();
            FinansoData.RepositoryResult<IEnumerable<Tuple<int, string>>> transactionTypes = await _transactionMetaQueryRepository.GetShortListOfAllTransactionTypes();

            FinansoData.RepositoryResult<IEnumerable<Tuple<int, string>>> balances = await _balanceQueryRepository.GetShortListOfBalanceForUser(userName);

            if (transactionStatuses.IsSuccess == false
                || transactionTypes.IsSuccess == false
                || balances.IsSuccess == false)
            {
                addTransactionViewModel.Error.GetDataInternalError = true;
                return addTransactionViewModel;
            }

            FinansoData.RepositoryResult<IEnumerable<Tuple<int, string>>> incomeCategories = await _transactionMetaQueryRepository.GetTransactionIncomeCategories();
            FinansoData.RepositoryResult<IEnumerable<Tuple<int, string>>> expenseCategories = await _transactionMetaQueryRepository.GetTransactionExpenseCategories();

            if (incomeCategories.IsSuccess == false || expenseCategories.IsSuccess == false)
            {
                addTransactionViewModel.Error.GetDataInternalError = true;
                return addTransactionViewModel;
            }

            // Map tuple from repo to SelectListItem
            addTransactionViewModel.TransactionStatuses = _mapper.Map<IEnumerable<Tuple<int, string>>, IEnumerable<SelectListItem>>(transactionStatuses.Value);
            addTransactionViewModel.TransactionTypes = _mapper.Map<IEnumerable<Tuple<int, string>>, IEnumerable<SelectListItem>>(transactionTypes.Value);
            addTransactionViewModel.Balances = _mapper.Map<IEnumerable<Tuple<int, string>>, IEnumerable<SelectListItem>>(balances.Value);
            addTransactionViewModel.TransactionIncomeCategories = _mapper.Map<IEnumerable<Tuple<int, string>>, IEnumerable<SelectListItem>>(incomeCategories.Value);
            addTransactionViewModel.TransactionExpenseCategories = _mapper.Map<IEnumerable<Tuple<int, string>>, IEnumerable<SelectListItem>>(expenseCategories.Value);


            if (balanceId.HasValue) addTransactionViewModel.BalanceId = (int)balanceId;
            return addTransactionViewModel;
        }

        private async Task<AddTransactionViewModel> GetDataForAddTransaction(int? balanceId, string userName, AddTransactionViewModel transactionViewModel)
        {
            AddTransactionViewModel addTransactionViewModel = await GetDataForAddTransaction(transactionViewModel.BalanceId, User.Identity.Name);
            transactionViewModel.TransactionStatuses = addTransactionViewModel.TransactionStatuses;
            transactionViewModel.TransactionTypes = addTransactionViewModel.TransactionTypes;
            transactionViewModel.Balances = addTransactionViewModel.Balances;

            return transactionViewModel;
        }

        #endregion

        [Authorize]
        [HttpGet]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByHeader = "Cookie", NoStore = false, VaryByQueryKeys = new[] { "pageNumber" })]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = await _settingsQueryRepository.GetSettingsAsync<int>("TransactionListPageSize");

            FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Transaction.GetTransactionsForUser>> data = await _transactionQueryRepository.GetTransactionsForUserUser(User.Identity.Name, pageNumber, pageSize);
            int pagesCount = (int)Math.Ceiling((double)data.TotalResult / pageSize);

            TransactionListViewModel transactionListViewModel = _mapper.Map<TransactionListViewModel>(data);

            transactionListViewModel.CurrentPage = pageNumber;
            transactionListViewModel.PagesCount = pagesCount;

            return View(transactionListViewModel);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddTransaction(int? balanceId, bool isRecurring)
        {
            AddTransactionViewModel addTransactionViewModel = await GetDataForAddTransaction(balanceId, User.Identity.Name);
            addTransactionViewModel.IsRecurring = isRecurring;
            return View(addTransactionViewModel);
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTransaction(AddTransactionViewModel transactionViewModel)
        {
            // Check if user has access to balance
            if (User.Identity?.Name == null)
            {
                return Unauthorized();
            }

            FinansoData.RepositoryResult<bool?> userAccess = await _balanceQueryRepository.HasUserAccessToBalance(User.Identity.Name, transactionViewModel.BalanceId);
            if (userAccess.Value != true)
            {
                return Unauthorized();
            }

            // Check if model is valid
            if (!ModelState.IsValid)
            {
                transactionViewModel = await GetDataForAddTransaction(transactionViewModel.BalanceId, User.Identity.Name, transactionViewModel);
                transactionViewModel.Error.WrongData = true;
                return View(transactionViewModel);
            }

            if(transactionViewModel.IsRecurring 
                && transactionViewModel.RecurringType is null)
            {
                return BadRequest();
            }

            int transactionCategoryId = 0;

            if (transactionViewModel.TransactionTypeId == 1)
            {
                transactionCategoryId = transactionViewModel.TransactionIncomeCategory;
            }
            else
            {
                transactionCategoryId = transactionViewModel.TransactionExpenseCategoryId;
            }

            FinansoData.RepositoryResult<bool> result = null;

            // Add transaction
            if (!transactionViewModel.IsRecurring)
            {
                if(transactionViewModel.TransactionStatusId is null)
                {
                    return BadRequest();
                }

                if (transactionViewModel.TransactionDate is null)
                {
                    transactionViewModel.TransactionDate = DateTime.Now;
                }

                result = await _transactionManagementRepository.AddTransaction(
                    transactionViewModel.Amount,
                    transactionViewModel.Description,
                    transactionViewModel.BalanceId,
                    (DateTime)transactionViewModel.TransactionDate,
                    User.Identity.Name,
                    transactionViewModel.TransactionTypeId,
                    (int)transactionViewModel.TransactionStatusId,
                    transactionCategoryId);
            }

            if (transactionViewModel.IsRecurring && transactionViewModel.RecurringType == "Weekly")
            {
                if(transactionViewModel.RecurringStartDate is null
                    && transactionViewModel.RecurringEndDate is null)
                {
                    return BadRequest();
                }

                result = await _transactionManagementRepository.AddTransactionWeeklyRecurring(
                    transactionViewModel.Amount,
                    transactionViewModel.Description,
                    transactionViewModel.BalanceId,
                    (DateTime)transactionViewModel.RecurringStartDate,
                    (DateTime)transactionViewModel.RecurringEndDate,
                    User.Identity.Name,
                    transactionViewModel.TransactionTypeId,
                    transactionCategoryId);
            }

            if (transactionViewModel.IsRecurring && transactionViewModel.RecurringType == "Monthly")
            {
                if (transactionViewModel.RecurringStartDate is null
                    && transactionViewModel.RecurringEndDate is null)
                {
                    return BadRequest();
                }

                result = await _transactionManagementRepository.AddTransactionMonthlyRecurring(
                    transactionViewModel.Amount,
                    transactionViewModel.Description,
                    transactionViewModel.BalanceId,
                    (DateTime)transactionViewModel.RecurringStartDate,
                    (DateTime)transactionViewModel.RecurringEndDate,
                    User.Identity.Name,
                    transactionViewModel.TransactionTypeId,
                    transactionCategoryId);
            }

            if (result is null)
            {
                return BadRequest();
            }

            if (result.IsSuccess == false ||
                result.ErrorType == FinansoData.ErrorType.NoUserFound)
            {
                return Unauthorized();
            }

            if (result.IsSuccess == false ||
                result.ErrorType == FinansoData.ErrorType.ServerError)
            {
                transactionViewModel = await GetDataForAddTransaction(transactionViewModel.BalanceId, User.Identity.Name, transactionViewModel);
                return View(transactionViewModel);
            }

            return RedirectToAction("Index");
        }
    }
}
