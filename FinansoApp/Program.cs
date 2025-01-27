using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using FinansoData.Repository.Account;
using FinansoData.Repository.Balance;
using FinansoData.Repository.Currency;
using FinansoData.Repository.Group;
using FinansoData.Repository.Settings;
using FinansoData.Repository.Transaction;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Scopes
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ISeed, Seed>();
builder.Services.AddScoped<IGroupCrudRepository, GroupCrudRepository>();
builder.Services.AddScoped<IGroupManagementRepository, GroupManagementRepository>();
builder.Services.AddScoped<IGroupQueryRepository, GroupQueryRepository>();
builder.Services.AddScoped<IGroupUsersQueryRepository, GroupUsersQuery>();
builder.Services.AddScoped<IGroupUsersManagementRepository, GroupUsersManagementRepository>();
builder.Services.AddScoped<IUserQuery, UserQuery>();
builder.Services.AddScoped<ICurrencyQueryRepository, CurrencyQueryRepository>();
builder.Services.AddScoped<IBalanceSumAmount, BalanceSumAmount>();

builder.Services.AddScoped<IBalanceManagmentRepository, BalanceManagementRepository>();
builder.Services.AddScoped<IBalanceQueryRepository, BalanceQueryRepository>();


builder.Services.AddScoped<ICacheWrapper, CacheWrapper>();
builder.Services.AddScoped<ISettingsQueryRepository, SettingsQueryRepository>();

// Repository account
builder.Services.AddScoped<IAuthentication, Authentication>();
builder.Services.AddScoped<IUserManagement, UserManagement>();

// Repository transaction
builder.Services.AddScoped<ITransactionsQueryRepository, TransactionsQueryRepository>();
builder.Services.AddScoped<ITransactionManagementRepository, TransactionManagementRepository>();
builder.Services.AddScoped<ITransactionMetaQueryRepository, TransactionMetaQueryRepository>();


// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 2;
});



builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
WebApplication app = builder.Build();




if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    string defaultPassword = builder.Configuration.GetValue<string>("DefaultPassword");
    Console.WriteLine($"Default password: \"{defaultPassword}\"");


    Console.WriteLine("Seeding data");

    Seed.SeedSettings(app);

    Console.WriteLine("Settings seeded");

    Seed.SeedTransactionTypes(app);

    Console.WriteLine("Transaction types seeded");

    Seed.SeedTransactionStatuses(app);

    Console.WriteLine("Transaction statuses seeded");

    Seed.SeedRoles(app);

    Console.WriteLine("User roles seeded");

    Seed.SeedCurrencies(app);

    Console.WriteLine("Currencies seeded");

    //Seed.SeedUsers(app, defaultPassword);
    Seed.SeedUsers(app, defaultPassword);

    Console.WriteLine("Users seeded");

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
