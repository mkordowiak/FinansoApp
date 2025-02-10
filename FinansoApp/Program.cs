using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<FinansoData.Repository.Balance.IBalanceManagmentRepository, FinansoData.Repository.Balance.BalanceManagementRepository>();
builder.Services.AddScoped<FinansoData.Repository.Balance.IBalanceQueryRepository, FinansoData.Repository.Balance.BalanceQueryRepository>();


builder.Services.AddScoped<ICacheWrapper, CacheWrapper>();
builder.Services.AddScoped<FinansoData.Repository.Settings.ISettingsQueryRepository, FinansoData.Repository.Settings.SettingsQueryRepository>();
builder.Services.AddScoped<FinansoData.Data.ITimedActions, FinansoData.Data.TimedActions>();

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



builder.Services.AddDbContext<FinansoData.Data.ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#if DEBUG
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
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
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 2;
});
#endif



#region hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new Hangfire.SqlServer.SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();
#endregion

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<FinansoData.Data.ApplicationDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
WebApplication app = builder.Build();


#region Hangfire Recurring Job Scheduling (po app.Build())
IRecurringJobManager recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<FinansoData.Data.TimedActions>(
    "aaaaa",
    job => job.UpdateBalanceTransactions(),
    Cron.Daily(21)
);
#endregion




if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    string defaultPassword = builder.Configuration.GetValue<string>("DefaultPassword");
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
