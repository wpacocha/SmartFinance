using Xunit;
using Microsoft.AspNetCore.Mvc;
using SmartFinance.API.Controllers;
using SmartFinance.API.Data;
using SmartFinance.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.InMemory;


public class ReportControllerTests
{
    [Fact]
    public void GetMonthlySummary_ReturnsCorrectValues()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase("GetMonthlySummary")
            .Options;

        using (var context = new FinanceDbContext(options))
        {
            context.Transactions.AddRange(new List<Transaction>
            {
                new Transaction { Amount = 1000, IsIncome = true, UserId = 1, Month = 3, Year = 2025 },
                new Transaction { Amount = 300, IsIncome = false, UserId = 1, Month = 3, Year = 2025 }
            });
            context.SaveChanges();

            var controller = new ReportController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier, "1")
                    }, "mock"))
                }
            };

            var result = controller.GetMonthlySummary(3, 2025);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}