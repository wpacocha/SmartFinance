using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SmartFinance.API.Models;
using SmartFinance.API.Data;
using System.Security.Claims;
using SmartFinance.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SmartFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public TransactionController(FinanceDbContext context)
    {
        _context = context;
    }
    
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public IActionResult GetTransactions()
    {
        var userId = GetUserId();
        var transactions = _context.Transactions
            .Where(t=>t.UserId == userId)
            .Include(t=>t.Category)
            .ToList();
        return Ok(transactions);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction,
        [FromServices] ExchangeRateService exchangeService)
    {
        transaction.UserId = GetUserId();

        // if user does not change the date set current
        if (transaction.Date == default)
            transaction.Date = DateTime.Now;

        transaction.Month = transaction.Date.Month;
        transaction.Year = transaction.Date.Year;

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok(transaction);
    }


    
    [HttpPut("{id}")]
    public IActionResult UpdateTransaction(int id, [FromBody] Transaction updatedTransaction)
    {
        var userId = GetUserId();
        var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id && t.UserId == userId);
        
        if (transaction == null)
            return NotFound("Transaction not found.");
        
        transaction.Description = updatedTransaction.Description;
        transaction.Amount = updatedTransaction.Amount;
        transaction.Date = updatedTransaction.Date;
        transaction.CategoryId = updatedTransaction.CategoryId;
        transaction.Currency = updatedTransaction.Currency;
        
        _context.SaveChanges();
        return Ok(transaction);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTransaction(int id)
    {
        var userId = GetUserId();
        var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id && t.UserId == userId);
        
        if (transaction == null)
            return NotFound("Transaction not found.");
        
        _context.Transactions.Remove(transaction);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        var userId = GetUserId();
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        
        var transactions = _context.Transactions
            .Where(t => t.UserId == userId)
            .ToList();
        
        var income = transactions
            .Where(t => t.IsIncome)
            .Sum(t => t.Amount);
        
        var expenses = transactions
            .Where(t=>!t.IsIncome)
            .Sum(t => t.Amount);
        
        var balance = income - expenses;

        return Ok(new
        {
            currency = user.PrefferedCurrency,
            income = Math.Round(income, 2),
            expenses = Math.Round(expenses, 2),
            balance = Math.Round(balance, 2)
        });
    }
    
    [HttpGet("monthly")]
    public IActionResult GetMonthly([FromQuery] int? month, [FromQuery] int? year)
    {
        var userId = GetUserId();
        var now = DateTime.Now;

        int targetMonth = month ?? now.Month;
        int targetYear = year ?? now.Year;

        var transactions = _context.Transactions
            .Where(t => t.UserId == userId && t.Month == targetMonth && t.Year == targetYear)
            .Include(t => t.Category)
            .ToList();

        return Ok(transactions);
    }

}