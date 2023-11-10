using FinansoData.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FinansoData.BLL;
using System.Net;

namespace FinansoData.Data
{
    public class Seed : ISeed
    {
        private readonly IAccountBLL _accountBLL;

        public Seed(IAccountBLL accountBLL)
        {
            this._accountBLL = accountBLL;
        }


        public async Task SeedData()
        {
            await _accountBLL.CreateAdminAsync("admin1", "admin1@mail.pl", "Haslo?123");
            await _accountBLL.CreateUserAsync("user1", "user1@mail.pl", "Haslo?123");
        }

        public static async Task SeedUsers(IApplicationBuilder applicationBuilder, string password)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Create roles
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                // Create users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string adminUserEmail = "admin1@gmail.com";


                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new AppUser()
                    {
                        UserName = "admin1",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                        Created = DateTime.Now

                    };
                    await userManager.CreateAsync(newAdminUser, password);
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                string appUserEmail = "user@user1.pl";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new AppUser()
                    {
                        UserName = "user1",
                        Email = appUserEmail,
                        EmailConfirmed = true,
                        Created = DateTime.Now
                    };
                    await userManager.CreateAsync(newAppUser, password);
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
            }
        }
    }
}
