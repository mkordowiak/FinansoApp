using FinansoData.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<BalanceTransaction>()
                .HasOne(a => a.Currency)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            

        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<BalanceTransaction> BalanceTransactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
    }
}
