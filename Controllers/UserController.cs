using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartFinance.API.Models;
using SmartFinance.API.Data;
using System.Security.Claims;

namespace SmartFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public UserController(FinanceDbContext context)
    {
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("me")]
    public IActionResult GetMySettings()
    {
        var userId = GetUserId();
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return NotFound();

        return Ok(new UserSettingsDto
        {
            Username = user.Username,
            PreferredCurrency = user.PreferredCurrency
        });
    }

    [HttpPut("currency")]
    public IActionResult UpdatePreferredCurrency([FromBody] string newCurrency)
    {
        var userId = GetUserId();
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return NotFound("User not found");

        user.PreferredCurrency = newCurrency.ToUpper();
        _context.SaveChanges();

        return Ok(new
        {
            message = "Currency updated.",
            currency = user.PreferredCurrency
        });
    }
}