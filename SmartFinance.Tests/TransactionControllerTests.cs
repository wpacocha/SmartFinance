using Xunit;
using Moq;
using SmartFinance.API.Controllers;
using SmartFinance.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartFinance.API.Models;
using SmartFinance.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

public class TransactionControllerTests
{
    private readonly TransactionController _controller;
    private readonly FinanceDbContext _context;

    public TransactionControllerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new FinanceDbContext(options);
        _controller = new TransactionController(_context);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
        }));
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public void GetTransactions_ReturnsOk()
    {
        var result = _controller.GetTransactions();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsOk()
    {
        var transaction = new Transaction { Description = "Test", Amount = 100, Currency = "USD" };
        var exchangeServiceMock = new Mock<ExchangeRateService>(MockBehavior.Loose, null);

        var result = await _controller.CreateTransaction(transaction, exchangeServiceMock.Object);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void DeleteTransaction_NotFound_WhenTransactionDoesNotExist()
    {
        var result = _controller.DeleteTransaction(999);
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
