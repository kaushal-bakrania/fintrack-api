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
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountDto dto)
        {
            if (!Enum.TryParse<AccountType>(dto.Type, out var accountType))
                return BadRequest("Invalid account type. Use 'Current' or 'Savings'.");

            var account = new Account
            {
                Name = dto.Name,
                Type = accountType,
                UserId = GetUserId()
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(new AccountResponseDto
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type.ToString(),
                Balance = account.Balance,
                CreatedAt = account.CreatedAt
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _context.Accounts
                .Where(a => a.UserId == GetUserId())
                .Select(a => new AccountResponseDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Type = a.Type.ToString(),
                    Balance = a.Balance,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == GetUserId());

            if (account == null)
                return NotFound("Account not found.");

            return Ok(new AccountResponseDto
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type.ToString(),
                Balance = account.Balance,
                CreatedAt = account.CreatedAt
            });
        }
    }
}
