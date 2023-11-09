using FinansoApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FinansoData.Repository;

namespace FinansoApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBalanceRepository _balanceRepository;

        public HomeController(ILogger<HomeController> logger, IBalanceRepository balanceRepository)
        {
            _logger = logger;
            this._balanceRepository = balanceRepository;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _balanceRepository.GetAllBalancesAsync();
            var data2 = await _balanceRepository.GetBalanceAsync(1);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}