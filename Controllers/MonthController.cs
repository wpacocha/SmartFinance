using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartFinance.API.Data;
using SmartFinance.API.Models;

namespace SmartFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MonthController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public MonthController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMonth([FromBody] Month month)
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();
        if (month.MonthNumber < 1 || month.MonthNumber > 12)
            return BadRequest("Invalid month number.");
        var exists = _context.Months.Any(m => m.UserId == userId && m.Year == month.Year && m.MonthNumber == month.MonthNumber);
        if (exists)
            return BadRequest("This month already exists.");


        var newMonth = new Month
        {
            UserId = userId,
            MonthNumber = month.MonthNumber, // <- poprawka, tylko MonthNumber
            Year = month.Year
        };

        _context.Months.Add(newMonth);
        await _context.SaveChangesAsync();

        return Ok(newMonth);
    }



    [HttpGet]
    public IActionResult GetMonths()
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        return Ok(_context.Months
            .Where(m => m.UserId == userId)
            .Select(m => new
            {
                m.Id,
                m.Year,
                Month = m.MonthNumber
            }).ToList());
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMonth(int id)
    {
        var month = await _context.Months.FindAsync(id);
        if (month == null) return NotFound();

        _context.Months.Remove(month);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
