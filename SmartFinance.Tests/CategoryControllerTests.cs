using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartFinance.API.Controllers;
using SmartFinance.API.Data;
using SmartFinance.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class CategoryControllerTests
{
    private CategoryController GetControllerWithMockContext(List<Category> data, int userId = 1)
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new FinanceDbContext(options);
        context.Categories.AddRange(data);
        context.SaveChanges();

        var controller = new CategoryController(context);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    [Fact]
    public void GetCategories_ReturnsCategoriesForUser()
    {
        // Arrange
        var categories = new List<Category>
    {
        new Category { Id = 1, Name = "Food", UserId = 1 },
        new Category { Id = 2, Name = "Global", UserId = null }
    };
        var controller = GetControllerWithMockContext(categories);

        // Act
        var result = controller.GetCategories();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        // Assert
        var returned = Assert.IsType<List<Category>>(okResult.Value);
        Assert.Equal(2, returned.Count);
    }

    [Fact]
    public void CreateCategory_SavesToDatabase()
    {
        // Arrange
        var controller = GetControllerWithMockContext(new List<Category>());
        var newCategory = new Category { Name = "Travel" };

        // Act
        var result = controller.CreateCategory(newCategory) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        var created = Assert.IsType<Category>(result.Value);
        Assert.Equal("Travel", created.Name);
        Assert.Equal(1, created.UserId);
    }

    [Fact]
    public void DeleteCategory_RemovesCorrectCategory()
    {
        // Arrange
        var categories = new List<Category> { new Category { Id = 1, Name = "Bills", UserId = 1 } };
        var controller = GetControllerWithMockContext(categories);

        // Act
        var result = controller.DeleteCategory(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        var controller = GetControllerWithMockContext(new List<Category>());
        var result = controller.DeleteCategory(999);
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
