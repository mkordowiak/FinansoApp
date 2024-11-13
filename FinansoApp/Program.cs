using FinansoApp.Controllers;
using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FinansoData.Repository.Account;
using FinansoData.Repository.Group;

var builder = WebApplication.CreateBuilder(args);

// Scopes
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IBalanceRepository, BalanceRepository>();
builder.Services.AddScoped<ISeed, Seed>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IGroupCrudRepository, GroupCrudRepository>();
builder.Services.AddScoped<IGroupManagementRepository, GroupManagementRepository>();
builder.Services.AddScoped<IGroupQueryRepository, GroupQueryRepository>();
builder.Services.AddScoped<IGroupUsersQueryRepository, GroupUsersQuery>();
builder.Services.AddScoped<IGroupUsersManagementRepository, GroupUsersManagementRepository>();
builder.Services.AddScoped<IUserQuery, UserQuery>();

// Repository account
builder.Services.AddScoped<IAuthentication, Authentication>();
builder.Services.AddScoped<IUserManagement, UserManagement>();



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
var app = builder.Build();




if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    string defaultPassword = builder.Configuration.GetValue<string>("DefaultPassword");
    Seed.SeedUsers(app, defaultPassword);
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
