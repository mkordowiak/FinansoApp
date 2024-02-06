using FinansoApp.Models;
using FinansoData.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FinansoApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISeed _seed;

        public HomeController(ILogger<HomeController> logger, ISeed seed)
        {
            _logger = logger;
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