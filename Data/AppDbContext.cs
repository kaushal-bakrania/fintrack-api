using Microsoft.EntityFrameworkCore;
using FinTrackAPI.Models;

namespace FinTrackAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MonthlyReport> MonthlyReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User → Accounts (one to many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Account → Transactions (one to many)
            modelBuilder.Entity<Account>()
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → MonthlyReports (one to many)
            modelBuilder.Entity<User>()
                .HasMany<MonthlyReport>()
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Decimal precision for money fields
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MonthlyReport>()
                .Property(r => r.TotalIncome)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MonthlyReport>()
                .Property(r => r.TotalExpenses)
                .HasColumnType("decimal(18,2)");
        }
    }
}