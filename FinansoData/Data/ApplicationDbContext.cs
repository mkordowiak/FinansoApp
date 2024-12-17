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
                .Entity<BalanceTransaction>()
                .HasOne(t => t.Balance)
                .WithMany(bt => bt.BalanceTransactions)
                .HasForeignKey(bt => bt.BalanceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<BalanceTransaction>()
                .HasOne(t => t.Group)
                .WithMany(g => g.BalanceTransactions)
                .HasForeignKey(bt => bt.GroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<BalanceTransaction>()
                .HasOne(t => t.AppUser)
                .WithMany(u => u.BalanceTransactions)
                .HasForeignKey(bt => bt.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<BalanceTransaction>()
                .HasOne(t => t.TransactionType)
                .WithMany(tt => tt.BalanceTransactions)
                .HasForeignKey(bt => bt.TransactionTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<BalanceTransaction>()
                .HasOne(t => t.TransactionStatus)
                .WithMany(ts => ts.BalanceTransactions)
                .HasForeignKey(bt => bt.TransactionStatusId)
                .OnDelete(DeleteBehavior.NoAction);




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






            // Indexes
            modelBuilder
                .Entity<GroupUser>()
                .HasIndex(gu => new { gu.AppUserId, gu.GroupId })
                .IsUnique();

            modelBuilder
                .Entity<BalanceTransaction>()
                .HasIndex(bt => new { bt.TransactionDate });
            modelBuilder
                .Entity<BalanceTransaction>()
                .HasIndex(bt => new { bt.BalanceId });
            modelBuilder
                .Entity<BalanceTransaction>()
                .HasIndex(bt => new { bt.GroupId, bt.AppUserId });

            modelBuilder
                .Entity<AppUser>()
                .HasIndex(u => new { u.NormalizedUserName });


            // Default values
            modelBuilder
                .Entity<Group>()
                .Property(g => g.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder
                .Entity<GroupUser>()
                .Property(gu => gu.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder
                .Entity<AppUser>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder
                .Entity<Balance>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETDATE()");


            modelBuilder
                .Entity<BalanceTransaction>()
                .Property(bt => bt.CreatedAt)
                .HasDefaultValueSql("GETDATE()");


            

        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<BalanceTransaction> BalanceTransactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<TransactionStatus> TransactionStatuses { get; set; }
        public DbSet<Settings> Settings { get; set; }
    }
}
