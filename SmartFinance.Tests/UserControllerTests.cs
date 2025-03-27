using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SmartFinance.API.Controllers;
using SmartFinance.API.Data;
using SmartFinance.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartFinance.Tests.Controllers
{
    public class UserControllerTests
    {
        private UserController GetControllerWithContext(FinanceDbContext context, int userId = 1)
        {
            var controller = new UserController(context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            return controller;
        }

        [Fact]
        public void GetMySettings_ReturnsUserSettings_WhenUserExists()
        {
            // Arrange
            var db = TestDbContextFactory.CreateDbContext();
            db.Users.Add(new User { Id = 1, Username = "john", PreferredCurrency = "USD", PasswordHash = new byte[1], PasswordSalt = new byte[1] });
            db.SaveChanges();

            var controller = GetControllerWithContext(db);

            var result = controller.GetMySettings() as OkObjectResult;
            var settings = result.Value as UserSettingsDto;
            Assert.NotNull(settings);
            Assert.Equal("john", settings.Username);
            Assert.Equal("USD", settings.PreferredCurrency);

        }

        [Fact]
        public void GetMySettings_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var db = TestDbContextFactory.CreateDbContext();
            var controller = GetControllerWithContext(db);

            // Act
            var result = controller.GetMySettings();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdatePreferredCurrency_UpdatesCurrency_WhenUserExists()
        {
            // Arrange
            var db = TestDbContextFactory.CreateDbContext();
            db.Users.Add(new User { Id = 1, Username = "john", PreferredCurrency = "USD", PasswordSalt = new byte[1], PasswordHash = new byte[1] });
            db.SaveChanges();

            var controller = GetControllerWithContext(db);

            // Act
            var result = controller.UpdatePreferredCurrency("eur") as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var updatedUser = db.Users.First(u => u.Id == 1);
            Assert.Equal("EUR", updatedUser.PreferredCurrency);
        }

        [Fact]
        public void UpdatePreferredCurrency_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var db = TestDbContextFactory.CreateDbContext();
            var controller = GetControllerWithContext(db);

            // Act
            var result = controller.UpdatePreferredCurrency("eur");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
