using Microsoft.EntityFrameworkCore;
using FinTrackAPI.Data;
using FinTrackAPI.Models;

namespace FinTrackAPI.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task GenerateMonthlyReportsAsync()
        {
            var now = DateTime.UtcNow;
            var month = now.Month == 1 ? 12 : now.Month - 1;
            var year = now.Month == 1 ? now.Year - 1 : now.Year;

            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                // Skip if report already exists for this month
                var exists = await _context.MonthlyReports
                    .AnyAsync(r => r.UserId == user.Id && r.Month == month && r.Year == year);

                if (exists) continue;

                // Get all transactions for this user last month
                var transactions = await _context.Transactions
                    .Include(t => t.Account)
                    .Where(t => t.Account.UserId == user.Id
                        && t.Date.Month == month
                        && t.Date.Year == year)
                    .ToListAsync();

                var totalIncome = transactions
                    .Where(t => t.Type == TransactionType.Credit)
                    .Sum(t => t.Amount);

                var totalExpenses = transactions
                    .Where(t => t.Type == TransactionType.Debit)
                    .Sum(t => t.Amount);

                var report = new MonthlyReport
                {
                    UserId = user.Id,
                    Month = month,
                    Year = year,
                    TotalIncome = totalIncome,
                    TotalExpenses = totalExpenses
                };

                _context.MonthlyReports.Add(report);
            }

            await _context.SaveChangesAsync();
        }
    }
}
