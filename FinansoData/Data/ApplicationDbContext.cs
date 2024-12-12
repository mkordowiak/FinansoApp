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

            modelBuilder
                .Entity<GroupUser>()
                .HasIndex(gu => new { gu.AppUserId, gu.GroupId })
                .IsUnique();

            modelBuilder
                .Entity<GroupUser>()
                .HasOne(gu => gu.AppUser)
                .WithMany()
                .HasForeignKey(gu => gu.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<GroupUser>()
                .HasOne(gu => gu.Group)
                .WithMany(g => g.GroupUser)
                .HasForeignKey(gu => gu.GroupId)
                .OnDelete(DeleteBehavior.NoAction);







            modelBuilder
                .Entity<Balance>()
                .HasOne(c => c.Currency)
                .WithMany(b => b.Balance)
                .HasForeignKey(b => b.CurrencyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<Balance>()
                .HasOne(g => g.Group)
                .WithMany(b => b.Balance)
                .HasForeignKey(b => b.GroupId)
                .OnDelete(DeleteBehavior.NoAction);








            modelBuilder
                .Entity<Group>()
                .Property(g => g.Created)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder
                .Entity<GroupUser>()
                .Property(gu => gu.Created)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder
                .Entity<AppUser>()
                .Property(u => u.Created)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder
                .Entity<Balance>()
                .Property(b => b.Created)
                .HasDefaultValueSql("GETDATE()");


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
