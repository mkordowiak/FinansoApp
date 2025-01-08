using AutoMapper;
using FinansoApp.ViewModels.Transaction;
using FinansoData.Repository.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionsQueryRepository _transactionQueryRepository;
        private readonly ITransactionManagementRepository _transactionManagementRepository;
        private readonly IMapper _mapper;

        public TransactionController(
            ITransactionsQueryRepository transactionQueryRepository,
            ITransactionManagementRepository transactionManagementRepository,
            IMapper mapper)
        {
            _transactionQueryRepository = transactionQueryRepository;
            _transactionManagementRepository = transactionManagementRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        [ResponseCache(Duration = 20, Location = ResponseCacheLocation.Any, VaryByHeader = "Cookie", NoStore = false, VaryByQueryKeys = new[] { "page" })]
        public async Task<IActionResult> Index(int page = 1)
        {
            FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Transaction.GetTransactionsForUser>> data = await _transactionQueryRepository.GetTransactionsForUser(User.Identity.Name, page, 20);
            int pagesCount = (int)Math.Ceiling((double)data.TotalResult / 20);

            TransactionListViewModel transactionListViewModel  = _mapper.Map<TransactionListViewModel>(data);

            transactionListViewModel.CurrentPage = page;
            transactionListViewModel.PagesCount = pagesCount;

            return View(transactionListViewModel);
        }
    }
}
