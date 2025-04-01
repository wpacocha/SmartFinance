using Microsoft.AspNetCore.Mvc;
using SmartFinance.API.Data;
using SmartFinance.API.Models;
using SmartFinance.API.Services;

namespace SmartFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExchangeRateController : ControllerBase
{
    private readonly ExchangeRateService _service;
    private readonly FinanceDbContext _context;

    public ExchangeRateController(ExchangeRateService service, FinanceDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet("{currency}")]
    public async Task<IActionResult> GetRateAsync(string currency)
    {
        var rate = await _service.GetRateAsync(currency);
        if (rate == null)
            return NotFound($"Currency '{currency}' not found.");

        _context.ExchangeRates.Add(rate);
        await _context.SaveChangesAsync();

        return Ok(rate);
    }
}