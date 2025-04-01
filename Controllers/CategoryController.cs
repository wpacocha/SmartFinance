using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartFinance.API.Models;
using SmartFinance.API.Data;

namespace SmartFinance.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public CategoryController(FinanceDbContext context)
    {
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public ActionResult<IEnumerable<Category>> GetCategories()
    {
        var userId = GetUserId();
        var categories = _context.Categories
            .Where(c => c.UserId == null || c.UserId == userId)
            .ToList();
        return Ok(categories);
    }

    [HttpPost]
    public IActionResult CreateCategory([FromBody] Category category)
    {
        category.UserId = GetUserId();
        _context.Categories.Add(category);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(int id)
    {
        var userId = GetUserId();
        var category = _context.Categories.FirstOrDefault(c => c.Id == id && c.UserId == userId);
        if (category == null)
            return NotFound("Category not found.");
        _context.Categories.Remove(category);
        _context.SaveChanges();
        return NoContent();
    }
}