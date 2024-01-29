using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FinansoData.Data;
using FinansoData.Models;

namespace FinansoApp.Tests.Controllers
{
    public class AccountControllerTest
    {
        private readonly UserManager<AppUser> _userManager;
        public AccountControllerTest()
        {
            
        }
        [Fact]
        public void AccountController_Login_ShoudLoginWhenCredentialsAreCorrect()
        {
            var www = new DBContextBuilder<ApplicationDbContext>();
        }
    }
}
