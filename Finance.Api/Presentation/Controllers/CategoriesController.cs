using Finance.Api.Application.DTOs.Category;
using Finance.Api.Models;
using Finance.Api.Data;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Helpers;
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

    //------------------------------------------------------------------------------------------------------------------------------------
    //create category endpoint

    [HttpPost("create")]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto request)
    {
        var userId = User.GetUserId();

        // Check if category with same name already exists for this user
        var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == request.Name.ToLower().Trim() && (c.IsSystemCategory||c.UserId==userId));
        if (existingCategory != null)
        {
            return BadRequest(new ApiResponse { Success = false, Message = "Category with this name already exists" });
        }

        var category = new Category
        {
            Name = request.Name.Trim(),
            UserId = userId,
            IsSystemCategory = false
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name, IsSystemCategory = category.IsSystemCategory };
        var response = new ApiResponse<CategoryDto> { Success = true, Message = "Category created successfully", Data = categoryDto };
        return CreatedAtAction(nameof(CreateCategory), response);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //get all categories for the user

    [HttpGet("get")]
    public async Task<IActionResult> GetCategories()
    {
        var userId = User.GetUserId();
        var categories = await _context.Categories.AsNoTracking().Where(c => (c.IsSystemCategory||c.UserId==userId)).ToListAsync();
        var categoryDtos = categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, IsSystemCategory = c.IsSystemCategory }).ToList();
        return Ok(categoryDtos);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //update category endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto request)
    {
        var userId = User.GetUserId();
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && (c.IsSystemCategory||c.UserId == userId));
        if (category == null)
        {
            return NotFound(new ApiResponse { Success = false, Message = "Category not found" });
        }

        //check if category is system category
        if (category.IsSystemCategory)
        {
            return BadRequest("Can not update pre-defined categories");
        }

        // Check if new name is duplicate (case-insensitive, excluding current category)
        var isDuplicate = await _context.Categories.AnyAsync(c => (c.IsSystemCategory || c.UserId == userId) && c.Name.ToLower().Trim() == request.Name.ToLower().Trim() && c.Id != id);
        if (isDuplicate)
        {
            return BadRequest(new ApiResponse { Success = false, Message = "Category with this name already exists" });
        }

        category.Name = request.Name;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name, IsSystemCategory=category.IsSystemCategory };
        var response = new ApiResponse<CategoryDto> { Success = true, Message = "Category updated successfully", Data = categoryDto };
        return Ok(response);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //delete category endpoint

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var userId = User.GetUserId();
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && (c.IsSystemCategory||c.UserId == userId));
        if (category == null)
        {
            return NotFound(new ApiResponse { Success = false, Message = "Category not found" });
        }
        //check if category is not system category
        if (category.IsSystemCategory)
        {
            return BadRequest("Can not delete pre-defined categories");
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return Ok(new ApiResponse { Success = true, Message = "Category deleted successfully" });
    }
}