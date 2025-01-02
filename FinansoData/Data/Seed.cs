using FinansoData.Models;
using FinansoData.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FinansoData.Data
{
    public class Seed : ISeed
    {
        private readonly IAccountRepository _accountRepository;
        //ICurrencyRepository _currencyRepository;


        public Seed(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            //_currencyRepository = currencyRepository;
        }

        /// <summary>
        /// Static method to run ONLY when project is initialized
        /// Seed user roles
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void SeedRoles(IApplicationBuilder applicationBuilder)
        {
            using (IServiceScope serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                RoleManager<IdentityRole> roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                bool adminRoleExists;
                bool userRoleExists;

                Task.Run(async () =>
                {
                    adminRoleExists = await roleManager.RoleExistsAsync(UserRoles.Admin);
                    userRoleExists = await roleManager.RoleExistsAsync(UserRoles.User);

                    if (!adminRoleExists)
                    {
                        IdentityResult identityAdmin = await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                    }

                    if (!userRoleExists)
                    {
                        IdentityResult identityUser = await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                    }
                }).Wait();
            }
        }


        /// <summary>
        /// Static method to run ONLY when project is initialized
        /// Seed currencies
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void SeedCurrencies(IApplicationBuilder applicationBuilder)
        {
            using (IServiceScope serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                ApplicationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                if (!context.Currencies.Any())
                {
                    context.Currencies.AddRange(
                        new Currency { Code = "PLN", Name = "Polski złoty", ExchangeRate = 1, UpdatedAt = DateTime.Now },
                        new Currency { Code = "EUR", Name = "Euro", ExchangeRate = 4.5m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "USD", Name = "Dolar amerykański", ExchangeRate = 4.2m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "GBP", Name = "Funt brytyjski", ExchangeRate = 4.8m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "CHF", Name = "Frank szwajcarski", ExchangeRate = 4.1m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "JPY", Name = "Jen japoński", ExchangeRate = 3.9m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "CZK", Name = "Korona czeska", ExchangeRate = 3.7m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "SEK", Name = "Korona szwedzka", ExchangeRate = 3.6m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "NOK", Name = "Korona norweska", ExchangeRate = 3.5m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "DKK", Name = "Korona duńska", ExchangeRate = 3.4m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "CAD", Name = "Dolar kanadyjski", ExchangeRate = 3.3m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "AUD", Name = "Dolar australijski", ExchangeRate = 3.2m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "NZD", Name = "Dolar nowozelandzki", ExchangeRate = 3.1m, UpdatedAt = DateTime.Now },
                        new Currency { Code = "BTC", Name = "Bitcoin", ExchangeRate = 424069m, UpdatedAt = DateTime.Now }
                    );
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Static method to run ONLY when project is initialized
        /// Seed transaction types
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void SeedTransactionTypes(IApplicationBuilder applicationBuilder)
        {
            using (IServiceScope serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                ApplicationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                if (!context.TransactionTypes.Any())
                {
                    context.TransactionTypes.AddRange(
                        new TransactionType { Name = "Income" },
                        new TransactionType { Name = "Expense" }
                    );
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Static method to run ONLY when project is initialized
        /// Seed transaction statuses
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void SeedTransactionStatuses(IApplicationBuilder applicationBuilder)
        {
            using (IServiceScope serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                ApplicationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                if (!context.TransactionStatuses.Any())
                {
                    context.TransactionStatuses.AddRange(
                        new TransactionStatus { Name = "Planded" },
                        new TransactionStatus { Name = "Completed" },
                        new TransactionStatus { Name = "Canceled" }
                    );
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Static method to run ONLY when project is initialized
        /// Seed settings
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void SeedSettings(IApplicationBuilder applicationBuilder)
        {
            using (IServiceScope serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                ApplicationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                if (!context.Settings.Any())
                {
                    context.Settings.AddRange(
                        new Settings { Key = "MaxGroupUsersLimit", Value = "10", Type = "int", Description = "Limit of group users" }
                    );
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Static method to run ONLY when project is initialized
        /// Seed users
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="defaultPassword"></param>
        /// <returns></returns>
        public static async Task SeedUsers(IApplicationBuilder applicationBuilder, string defaultPassword)
        {
            using (IServiceScope serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                UserManager<AppUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();


                AppUser adminUser = new AppUser
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    UserName = "admin1@username.com",
                    Email = "admin1@username.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };

                Task.Run(async () =>
                {
                    AppUser? user = await userManager.FindByEmailAsync(adminUser.Email);
                    if (user == null)
                    {
                        IdentityResult result = await userManager.CreateAsync(adminUser, defaultPassword);
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
                        }
                    }
                }).Wait();


                List<AppUser> appUsers = new List<AppUser>
                {
                    new AppUser
                    {
                        FirstName = "User",
                        LastName = "1",
                        UserName = "user1@username.com",
                        Email = "user1@username.com",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now
                    },
                    new AppUser
                    {
                        FirstName = "User",
                        LastName = "2",
                        UserName = "user2@username.com",
                        Email = "user2@username.com",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now
                    }
                };

                foreach (AppUser user in appUsers)
                {
                    Task.Run(async () =>
                    {
                        AppUser? userInDb = await userManager.FindByEmailAsync(user.Email);

                        if (userInDb == null)
                        {
                            IdentityResult result = await userManager.CreateAsync(user, defaultPassword);
                            if (result.Succeeded)
                            {
                                await userManager.AddToRoleAsync(user, UserRoles.User);
                            }
                        }

                    }).Wait();
                }
            }
        }


        public Task<bool> SeedCurrencies()
        {
            throw new NotImplementedException();
        }
    }
}
