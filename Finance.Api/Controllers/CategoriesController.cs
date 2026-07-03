using Finance.Api.DTOs.Category;
using Finance.Api.Models;
using Finance.Api.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public CategoriesController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto request){
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var category = new Category{
            Name = request.Name,
            UserId = userId
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(){
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var categories = await _context.Categories.Where(c => c.UserId == userId).ToListAsync();
        return Ok(categories);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id){
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if(category == null){
            return NotFound();
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent(); 
    }
}