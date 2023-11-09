using FinansoData.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FinansoData.Data
{
    public class Seed
    {
        public static async Task SeedData(IApplicationBuilder applicationBuilder)
        {
            using (IServiceScope serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                ApplicationDbContext? context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                // Roles
                RoleManager<IdentityRole> roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                //{
                //    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                //}

                //if (!await roleManager.RoleExistsAsync(UserRoles.User))
                //{
                //    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                //}


                // Users
                UserManager<AppUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string adminUserEmail = "user@gmail.com";
                AppUser newUser = new AppUser()
                {
                    UserName = "user1",
                    Email = adminUserEmail,
                    EmailConfirmed = true,
                    FirstName = "user",
                    Created = DateTime.Now

                };
                await userManager.CreateAsync(newUser, "123123");
                //await userManager.AddToRoleAsync(newUser, UserRoles.Admin);
            }
        }
    }
}
