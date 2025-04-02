using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartFinance.API.Models;
using SmartFinance.API.Data;
using System.Security.Claims;
using SmartFinance.API.Services;
using System.Threading.Tasks;

namespace SmartFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly FinanceDbContext _context;
    private readonly ICurrencyConversionService _conversionService;

    public UserController(FinanceDbContext context, ICurrencyConversionService conversionService)
    {
        _context = context;
        _conversionService = conversionService;
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
    public async Task<IActionResult> UpdatePreferredCurrency([FromBody] UpdateCurrencyRequest request)
    {
        var userId = GetUserId();
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return NotFound("User not found");

        var newCurrency = request.Currency.ToUpper();

        await _conversionService.ConvertUserTransactionsToCurrency(user, newCurrency);

        user.PreferredCurrency = newCurrency;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Currency updated.",
            currency = user.PreferredCurrency
        });
    }

}