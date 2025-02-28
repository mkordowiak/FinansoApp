using FinansoData.Data;
using FinansoData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ISeed _seed;

        public AdminController(UserManager<AppUser> userManager, ISeed seed)
        {
            _userManager = userManager;
            _seed = seed;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SeedBasicData()
        {
            await _seed.SeedCurrencies();
            return RedirectToAction("Index", "Home");
        }
    }
}
