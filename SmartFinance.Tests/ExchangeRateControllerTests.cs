using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFinance.API.Controllers;
using SmartFinance.API.Data;
using SmartFinance.API.Models;
using SmartFinance.API.Services;
using System;
using System.Threading.Tasks;

public class FakeExchangeRateService : ExchangeRateService
{
    public FakeExchangeRateService() : base(new HttpClient()) { }
    public override Task<ExchangeRate?> GetRateAsync(string currency)
    {
        return Task.FromResult<ExchangeRate?>(new ExchangeRate
        {
            Currency = currency,
            Rate = 4.5m,
            Date = DateTime.Today
        });
    }
}

public class ExchangeRateControllerTests
{
    [Fact]
    public async Task GetRate_ReturnsExchangeRate()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new FinanceDbContext(options);
        var fakeService = new FakeExchangeRateService(); 

        var controller = new ExchangeRateController(fakeService, context);

        // Act
        var result = await controller.GetRate("USD");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<ExchangeRate>(okResult.Value);
        Assert.Equal("USD", returned.Currency);
    }
}
