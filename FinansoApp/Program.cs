using FinansoApp.Scheduled;
using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.AspNetCore;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region Register services
// Scopes
builder.Services.AddScoped<FinansoData.Data.ISeed, FinansoData.Data.Seed>();
builder.Services.AddScoped<FinansoData.Repository.Group.IGroupCrudRepository, FinansoData.Repository.Group.GroupCrudRepository>();
builder.Services.AddScoped<FinansoData.Repository.Group.IGroupManagementRepository, FinansoData.Repository.Group.GroupManagementRepository>();
builder.Services.AddScoped<FinansoData.Repository.Group.IGroupQueryRepository, FinansoData.Repository.Group.GroupQueryRepository>();
builder.Services.AddScoped<FinansoData.Repository.Group.IGroupUsersQueryRepository, FinansoData.Repository.Group.GroupUsersQuery>();
builder.Services.AddScoped<FinansoData.Repository.Group.IGroupUsersManagementRepository, FinansoData.Repository.Group.GroupUsersManagementRepository>();
builder.Services.AddScoped<FinansoData.Repository.Account.IUserQuery, FinansoData.Repository.Account.UserQuery>();
builder.Services.AddScoped<FinansoData.Repository.Currency.ICurrencyQueryRepository, FinansoData.Repository.Currency.CurrencyQueryRepository>();
builder.Services.AddScoped<FinansoData.Repository.Balance.IBalanceSumAmount, FinansoData.Repository.Balance.BalanceSumAmount>();

builder.Services.AddScoped<FinansoData.Repository.Balance.IBalanceManagementRepository, FinansoData.Repository.Balance.BalanceManagementRepository>();
builder.Services.AddScoped<FinansoData.Repository.Balance.IBalanceQueryRepository, FinansoData.Repository.Balance.BalanceQueryRepository>();


builder.Services.AddScoped<ICacheWrapper, CacheWrapper>();
builder.Services.AddScoped<FinansoData.Repository.Settings.ISettingsQueryRepository, FinansoData.Repository.Settings.SettingsQueryRepository>();
builder.Services.AddScoped<FinansoData.Repository.Procedures.ISumBalanceProcedure, FinansoData.Repository.Procedures.SumBalanceProcedure>();

// Repository account
builder.Services.AddScoped<FinansoData.Repository.Account.IAuthentication, FinansoData.Repository.Account.Authentication>();
builder.Services.AddScoped<FinansoData.Repository.Account.IUserManagement, FinansoData.Repository.Account.UserManagement>();

// Repository transaction
builder.Services.AddScoped<FinansoData.Repository.Transaction.ITransactionsQueryRepository, FinansoData.Repository.Transaction.TransactionsQueryRepository>();
builder.Services.AddScoped<FinansoData.Repository.Transaction.ITransactionManagementRepository, FinansoData.Repository.Transaction.TransactionManagementRepository>();
builder.Services.AddScoped<FinansoData.Repository.Transaction.ITransactionMetaQueryRepository, FinansoData.Repository.Transaction.TransactionMetaQueryRepository>();

// Repository chart
builder.Services.AddScoped<FinansoData.Repository.Chart.IChartDataRepository, FinansoData.Repository.Chart.ChartDataRepository>();


// Add services to the container.
builder.Services.AddControllersWithViews();
#endregion


builder.Services.AddDbContext<FinansoData.Data.ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#region Password settings
#if DEBUG
builder.Services.Configure<IdentityOptions>(options =>
{
    // debug password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 2;
    options.Password.RequiredUniqueChars = 1;
});
#else 
builder.Services.Configure<IdentityOptions>(options =>
{
    // Release assword settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 7;
    options.Password.RequiredUniqueChars = 2;
});
#endif
#endregion

#region Quartz.NET

builder.Services.AddQuartz(q =>
{
    JobKey jobKey = new JobKey("UpdateBalanceTransactionsAndUpdateBalances");
    q.AddJob<UpdateBalanceTransactionsAndUpdateBalances>(opts => opts.WithIdentity(jobKey));

#if DEBUG
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("UpdateBalanceTransactionsAndUpdateBalances-trigger")
        .WithCronSchedule("0 /2 * * * ?"));
#else
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("UpdateBalanceTransactionsAndUpdateBalances-trigger")
        .WithCronSchedule("0 /5 0-2 * * ?"));
#endif
});

builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

#endregion

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<FinansoData.Data.ApplicationDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
WebApplication app = builder.Build();



// If the app is in development mode, migrate the database and seed data
if (app.Environment.IsDevelopment())
{
    ApplicationDbContext application = app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

    IEnumerable<string> pendingMigrations = await application.Database.GetPendingMigrationsAsync();
    if (pendingMigrations != null)
    {
        await application.Database.MigrateAsync();
        seeddata();
    }

}

void seeddata()
{
    string? defaultPassword = builder.Configuration.GetValue<string>("DefaultPassword");

    if (string.IsNullOrEmpty(defaultPassword))
    {
        Console.WriteLine("Default password not found in appsettings.json. Exiting");
        return;
    }

    Console.WriteLine($"Default password: \"{defaultPassword}\"");


    Console.WriteLine("Seeding data");

    FinansoData.Data.Seed.SeedSettings(app);
    Console.WriteLine("Settings seeded");

    FinansoData.Data.Seed.SeedTransactionTypes(app);
    Console.WriteLine("Transaction types seeded");

    FinansoData.Data.Seed.SeedTransactionCategories(app);
    Console.WriteLine("Transaction categories seeded");

    FinansoData.Data.Seed.SeedTransactionStatuses(app);
    Console.WriteLine("Transaction statuses seeded");

    FinansoData.Data.Seed.SeedRoles(app);
    Console.WriteLine("User roles seeded");

    FinansoData.Data.Seed.SeedCurrencies(app);
    Console.WriteLine("Currencies seeded");

    //Seed.SeedUsers(app, defaultPassword);
    FinansoData.Data.Seed.SeedUsers(app, defaultPassword);

    Console.WriteLine("Users seeded");

    return;
}



if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    ApplicationDbContext application = app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    IEnumerable<string> pendingMigrations = await application.Database.GetPendingMigrationsAsync();
    if (pendingMigrations != null)
    {
        await application.Database.MigrateAsync();
    }

    seeddata();
}


if (args.Length == 1 && args[0].ToLower() == "migrate")
{
    Console.WriteLine("Migrating database");
    using IServiceScope scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;
    ApplicationDbContext context = services.GetRequiredService<FinansoData.Data.ApplicationDbContext>();
    context.Database.Migrate();
    Console.WriteLine("Database migrated");
    return;
}





// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();