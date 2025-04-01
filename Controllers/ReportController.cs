using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFinance.API.Data;
using SmartFinance.API.Models;
using System.Security.Claims;

namespace SmartFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public ReportController(FinanceDbContext context)
    {
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("monthly")]
    public IActionResult GetMonthlySummary([FromQuery] int? month, [FromQuery] int? year)
    {
        var userId = GetUserId();
        var now = DateTime.Now;

        int targetMonth = month ?? now.Month;
        int targetYear = year ?? now.Year;

        var transactions = _context.Transactions
            .Where(t => t.UserId == userId && t.Month == targetMonth && t.Year == targetYear)
            .ToList();

        var income = transactions.Where(t => t.IsIncome).Sum(t => t.Amount);
        var expenses = transactions.Where(t => !t.IsIncome).Sum(t => t.Amount);
        var balance = income - expenses;

        return Ok(new
        {
            month = targetMonth,
            year = targetYear,
            income = Math.Round(income, 2),
            expenses = Math.Round(expenses, 2),
            balance = Math.Round(balance, 2)
        });
    }

    [HttpGet("export")]
    public IActionResult ExportToCsv([FromQuery] int? month, [FromQuery] int? year)
    {
        var userId = GetUserId();
        var now = DateTime.Now;

        int targetMonth = month ?? now.Month;
        int targetYear = year ?? now.Year;

        var transactions = _context.Transactions
            .Where(t => t.UserId == userId && t.Month == targetMonth && t.Year == targetYear)
            .Include(t => t.Category)
            .ToList();

        var lines = new List<string>
    {
        "Date,Amount,Currency,IsIncome,Category,Description"
    };

        foreach (var t in transactions)
        {
            var line = $"{t.Date:yyyy-MM-dd},{t.Amount},{t.Currency},{t.IsIncome},{t.Category?.Name},{t.Description}";
            lines.Add(line);
        }

        var csvContent = string.Join("\n", lines);
        var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);

        return File(csvBytes, "text/csv", $"transactions_{targetYear}_{targetMonth}.csv");
    }

    [HttpGet("export-yearly")]
    public IActionResult ExportYearly([FromQuery] int year)
    {
        var userId = GetUserId();

        var transactions = _context.Transactions
            .Where(t => t.UserId == userId && t.Year == year)
            .Include(t => t.Category)
            .OrderBy(t => t.Date)
            .ToList();

        var lines = new List<string>
    {
        "Date,Amount,Currency,IsIncome,Category,Description"
    };

        foreach (var t in transactions)
        {
            var line = $"{t.Date:yyyy-MM-dd},{t.Amount},{t.Currency},{t.IsIncome},{t.Category?.Name},{t.Description}";
            lines.Add(line);
        }

        var csvContent = string.Join("\n", lines);
        var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);

        return File(csvBytes, "text/csv", $"transactions_{year}.csv");
    }

    [HttpGet("yearly")]
    public IActionResult GetYearlySummary([FromQuery] int year)
    {
        var userId = GetUserId();

        var transactions = _context.Transactions
            .Where(t => t.UserId == userId && t.Year == year)
            .ToList();

        var income = transactions.Where(t => t.IsIncome).Sum(t => t.Amount);
        var expenses = transactions.Where(t => !t.IsIncome).Sum(t => t.Amount);
        var balance = income - expenses;

        return Ok(new
        {
            year,
            income = Math.Round(income, 2),
            expenses = Math.Round(expenses, 2),
            balance = Math.Round(balance, 2)
        });
    }

}
