using ChartJSCore.Helpers;
using ChartJSCore.Models;
using FinansoApp.ViewModels.Balance;
using FinansoData.DataViewModel.Balance;
using FinansoData.Repository.Balance;
using FinansoData.Repository.Chart;
using FinansoData.Repository.Currency;
using FinansoData.Repository.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Controllers
{
    public class BalanceController : Controller
    {
        private readonly IBalanceManagementRepository _balanceManagementRepository;
        private readonly ICurrencyQueryRepository _currencyRepository;
        private readonly IGroupQueryRepository _groupQueryRepository;
        private readonly IGroupUsersQueryRepository _groupUsersQueryRepository;
        private readonly IBalanceQueryRepository _balanceQueryRepository;
        private readonly IBalanceSumAmount _balanceSumAmount;
        private readonly IChartDataRepository _chartDataRepository;

        public BalanceController(
            IBalanceManagementRepository balanceManagementRepository,
            ICurrencyQueryRepository currency,
            IGroupQueryRepository group,
            IGroupUsersQueryRepository groupUsersQueryRepository,
            IBalanceQueryRepository balanceQueryRepository,
            IBalanceSumAmount balanceSumAmount,
            IChartDataRepository chartDataRepository)
        {
            _balanceManagementRepository = balanceManagementRepository;
            _currencyRepository = currency;
            _groupQueryRepository = group;
            _groupUsersQueryRepository = groupUsersQueryRepository;
            _balanceQueryRepository = balanceQueryRepository;
            _balanceSumAmount = balanceSumAmount;
            _chartDataRepository = chartDataRepository;
            _balanceSumAmount = balanceSumAmount;
        }

        private string? userNameIdentity => User.Identity?.Name;

        private Chart GenerateVerticalBarChart(string chartTitle, List<string> labels, List<double?> numbers)
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Bar;

            Data data = new Data();


            data.Labels = labels;

            BarDataset dataset = new BarDataset()
            {
                Data = numbers,
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromRgba(255, 99, 132, 0.2),
                    ChartColor.FromRgba(54, 162, 235, 0.2),
                    ChartColor.FromRgba(255, 206, 86, 0.2),
                    ChartColor.FromRgba(75, 192, 192, 0.2),
                    ChartColor.FromRgba(153, 102, 255, 0.2),
                    ChartColor.FromRgba(255, 159, 64, 0.2)
                },
                BorderColor = new List<ChartColor>
                {
                    ChartColor.FromRgb(255, 99, 132),
                    ChartColor.FromRgb(54, 162, 235),
                    ChartColor.FromRgb(255, 206, 86),
                    ChartColor.FromRgb(75, 192, 192),
                    ChartColor.FromRgb(153, 102, 255),
                    ChartColor.FromRgb(255, 159, 64)
                },
                BorderWidth = new List<int>() { 1 },
                BarPercentage = 0.5,
                BarThickness = 100,
                MaxBarThickness = 150,
                MinBarLength = 2
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            Options options = new Options
            {
                Responsive = true,
                Plugins = new Plugins()
                {
                    Colors = new ColorPlugin()
                    {
                        Enabled = true
                    },
                    Legend = new Legend()
                    {
                        Display = false
                    },
                    Title = new Title()
                    {
                        Display = true,
                        Text = new List<string>() { chartTitle }
                    }
                },
                Scales = new Dictionary<string, Scale>()
                {
                    { "y", new CartesianLinearScale()
                        {
                            BeginAtZero = true,
                            Display = true,
                            //Ticks = new Tick{Padding = 100, Color = new ChartColor { Blue = 0, Green = 0, Red = 255, Alpha = 1 }, BackdropColor = new ChartColor { Blue = 0, Red = 0, Green = 255, Alpha = 1 } }

                        }
                    },
                    { "x", new Scale()
                        {
                            Grid = new Grid()
                            {
                                Offset = true
                            }
                        }
                    },
                }
            };

            chart.Options = options;

            chart.Options.Layout = new Layout
            {
                Padding = new Padding
                {
                    PaddingObject = new PaddingObject
                    {
                        Left = 10,
                        Right = 12
                    }
                }
            };

            return chart;
        }

        public async Task<IActionResult> Index()
        {
            if (userNameIdentity == null)
            {
                return Unauthorized();
            }

            // Get list of balances for user
            FinansoData.RepositoryResult<IEnumerable<BalanceViewModel>?> repositoryResult = await _balanceQueryRepository.GetListOfBalancesForUser(userNameIdentity);
            if (!repositoryResult.IsSuccess)
            {
                return BadRequest();
            }

            // Get sum of all balances for user
            FinansoData.RepositoryResult<decimal?> sumAmountResult = await _balanceSumAmount.GetBalancesSumAmountForUser(userNameIdentity);
            if (!sumAmountResult.IsSuccess)
            {
                BadRequest();
            }

            ViewModels.Balance.IndexViewModel indexViewModel = new ViewModels.Balance.IndexViewModel();
            indexViewModel.Balances = repositoryResult.Value ?? Enumerable.Empty<BalanceViewModel>();
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
            if (userNameIdentity == null)
            {
                return Unauthorized();
            }

            // Get all currencies and groups for current user
            FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel>> currencies = await _currencyRepository.GetAllCurrencies();
            FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>?> groups = await _groupQueryRepository.GetUserGroups(userNameIdentity);

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
            if (userNameIdentity == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                // Get all currencies and groups for current user
                balanceVM.Currencies = (await _currencyRepository.GetAllCurrencies()).Value;
                balanceVM.Groups = (await _groupQueryRepository.GetUserGroups(userNameIdentity)).Value;

                return View(balanceVM);
            }

            // Check if user has access to selected group
            FinansoData.RepositoryResult<FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel> groupMembershipResult = await _groupUsersQueryRepository.GetUserMembershipInGroupAsync(balanceVM.SelectedGroup, userNameIdentity);
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

            FinansoData.RepositoryResult<bool> repositoryResult = await _balanceManagementRepository.AddBalance(newBalanceViewModel);

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
            if (userNameIdentity == null)
            {
                return Unauthorized();
            }

            // Check if user has access to selected group
            FinansoData.RepositoryResult<bool?> hasUserAccessToGroup = await _balanceQueryRepository.HasUserAccessToBalance(userNameIdentity, id);

            if (!hasUserAccessToGroup.IsSuccess)
            {
                return BadRequest();
            }

            if (hasUserAccessToGroup.Value == false)
            {
                return Unauthorized();
            }


            // Chart data
            FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Chart.BalanceLogAverage>> balanceLogChartData = await _chartDataRepository.BalanceLogsByMonth(id);
            if (balanceLogChartData.IsSuccess)
            {
                Chart monthlyBalanceLogChart = GenerateVerticalBarChart("Balance monthly average", balanceLogChartData.Value.Select(x => $"{x.Month}-{x.Year}").ToList(), balanceLogChartData.Value.Select(x => (double?)x.Average).ToList());
                ViewData["BalanceMonthlyLog"] = monthlyBalanceLogChart;
            }

            FinansoData.RepositoryResult<BalanceViewModel> balance = await _balanceQueryRepository.GetBalance(id);

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

            if (userNameIdentity == null)
            {
                return Unauthorized();
            }

            // Check if user has access to selected group
            FinansoData.RepositoryResult<bool?> hasUserAccessToGroup = await _balanceQueryRepository.HasUserAccessToBalance(userNameIdentity, setBalanceAmountViewModel.BalanceId);

            if (!hasUserAccessToGroup.IsSuccess)
            {
                return BadRequest();
            }

            if (hasUserAccessToGroup.Value == false)
            {
                return Unauthorized();
            }


            FinansoData.RepositoryResult<bool?> repositoryResult = await _balanceManagementRepository.SetBalanceAmount(setBalanceAmountViewModel.BalanceId, setBalanceAmountViewModel.Amount);
            if (repositoryResult.IsSuccess == false)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Balance");
        }

        #endregion
    }
}
