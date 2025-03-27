using Xunit;
using Microsoft.AspNetCore.Mvc;
using SmartFinance.API.Controllers;
using SmartFinance.API.Models;
using SmartFinance.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ReturnsOk_WhenNewUser()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase("Auth_Register").Options;
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();

        using var context = new FinanceDbContext(options);
        var controller = new AuthController(context, config);

        var result = await controller.Register(new UserDto { Username = "test", Password = "123" });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenCredentialsValid()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase("Auth_Login").Options;

        var configData = new Dictionary<string, string> {
            { "Jwt:Key", "this_is_a_very_secure_key_that_is_long_enough_for_hmac_sha512_testing_1234567890" }
        };

   
        var config = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();

        using var context = new FinanceDbContext(options);
        var controller = new AuthController(context, config);

        await controller.Register(new UserDto { Username = "user", Password = "pass" });
        var result = await controller.Login(new UserDto { Username = "user", Password = "pass" });

        Assert.IsType<OkObjectResult>(result);
    }
}
