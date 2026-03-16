using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FinTrackAPI.Data;
using FinTrackAPI.DTOs;
using FinTrackAPI.Models;

namespace FinTrackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionDto dto)
        {
            // Verify account belongs to user
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == dto.AccountId && a.UserId == GetUserId());

            if (account == null)
                return NotFound("Account not found.");

            if (!Enum.TryParse<TransactionType>(dto.Type, out var transactionType))
                return BadRequest("Invalid type. Use 'Credit', 'Debit', or 'Transfer'.");

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            // Update account balance
            if (transactionType == TransactionType.Credit)
                account.Balance += dto.Amount;
            else if (transactionType == TransactionType.Debit)
            {
                if (account.Balance < dto.Amount)
                    return BadRequest("Insufficient funds.");
                account.Balance -= dto.Amount;
            }

            var transaction = new Transaction
            {
                AccountId = dto.AccountId,
                Type = transactionType,
                Amount = dto.Amount,
                Description = dto.Description
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new TransactionResponseDto
            {
                Id = transaction.Id,
                Type = transaction.Type.ToString(),
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                AccountId = account.Id,
                AccountName = account.Name
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] string? type,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount)
        {
            var query = _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.Account.UserId == GetUserId());

            // Filters
            if (!string.IsNullOrEmpty(type) && Enum.TryParse<TransactionType>(type, out var tType))
                query = query.Where(t => t.Type == tType);

            if (from.HasValue)
                query = query.Where(t => t.Date >= from.Value);

            if (to.HasValue)
                query = query.Where(t => t.Date <= to.Value);

            if (minAmount.HasValue)
                query = query.Where(t => t.Amount >= minAmount.Value);

            if (maxAmount.HasValue)
                query = query.Where(t => t.Amount <= maxAmount.Value);

            var transactions = await query
                .OrderByDescending(t => t.Date)
                .Select(t => new TransactionResponseDto
                {
                    Id = t.Id,
                    Type = t.Type.ToString(),
                    Amount = t.Amount,
                    Description = t.Description,
                    Date = t.Date,
                    AccountId = t.AccountId,
                    AccountName = t.Account.Name
                })
                .ToListAsync();

            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Id == id && t.Account.UserId == GetUserId());

            if (transaction == null)
                return NotFound("Transaction not found.");

            return Ok(new TransactionResponseDto
            {
                Id = transaction.Id,
                Type = transaction.Type.ToString(),
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                AccountId = transaction.AccountId,
                AccountName = transaction.Account.Name
            });
        }
    }
}

