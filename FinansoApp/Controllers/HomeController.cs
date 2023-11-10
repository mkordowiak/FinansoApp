using FinansoApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FinansoData.Repository;
using FinansoData.BLL;
using FinansoData.Data;

namespace FinansoApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountBLL _accountBLL;
        private readonly ISeed _seed;

        public HomeController(ILogger<HomeController> logger, IAccountBLL accountBLL, ISeed seed)
        {
            _logger = logger;
            _accountBLL = accountBLL;
            _seed = seed;
        }

        public async Task<IActionResult> Index()
        {
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