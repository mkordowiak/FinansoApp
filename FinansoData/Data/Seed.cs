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

        /// <summary>
        /// Static method to run ONLY when project is initialized
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="defaultPassword"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task SeedUsers(IApplicationBuilder applicationBuilder, string defaultPassword)
            {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Create roles if they dont exists
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

                    var adminUserCreation = await userManager.CreateAsync(newAdminUser, defaultPassword);

                    if (adminUserCreation.Succeeded == false)
                    {
                        throw new InvalidOperationException(adminUserCreation.Errors.ToString());
                    }

                    var adminUserRoleAssign = await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);

                    if(adminUserRoleAssign.Succeeded == false)
                    {
                        throw new InvalidOperationException(adminUserRoleAssign.Errors.ToString());
                    }
                }

                string appUserEmail = "user@user1.pl";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newRegularUser = new AppUser()
                {
                    UserName = "user1",
                        Email = appUserEmail,
                    EmailConfirmed = true,
                    FirstName = "user",
                    Created = DateTime.Now

                };
                    var regularUserCreation = await userManager.CreateAsync(newRegularUser, defaultPassword);
                    if (regularUserCreation.Succeeded == false)
                    {
                        throw new InvalidOperationException(regularUserCreation.Errors.ToString());
                    }
                    var regularUserRoleCreation = await userManager.AddToRoleAsync(newRegularUser, UserRoles.User);
                    if (regularUserRoleCreation.Succeeded == false)
                    {
                        throw new InvalidOperationException(regularUserRoleCreation.Errors.ToString());
                    }
                }
            }
        }
    }
}
