using Finance.Api.Application.Interfaces;
using Finance.Api.Data;
using Finance.Api.Application.DTOs.Category;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(FinanceDbContext context, ILogger<CategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(int userId, CreateCategoryDto request)
    {
        // var userId = User.GetUserId();

        // Check if category with same name already exists for this user
        var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == request.Name.ToLower().Trim() && (c.IsSystemCategory || c.UserId == userId)).ConfigureAwait(false);
        if (existingCategory != null)
        {
            _logger.LogWarning("Attempt to Create Category with Existing Name");
            return new ApiResponse<CategoryDto> { Success = false, Message = "Category with this name already exists" };
        }

        var category = new Category
        {
            Name = request.Name.Trim(),
            UserId = userId,
            IsSystemCategory = false
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        _logger.LogInformation("New Category Added by UserID : {userId}", userId);
        var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name, IsSystemCategory = category.IsSystemCategory };
        var response = new ApiResponse<CategoryDto> { Success = true, Message = "Category created successfully", Data = categoryDto };
        return response;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync(int userId)
    {
        var categories = await _context.Categories.AsNoTracking().Where(c => (c.IsSystemCategory || c.UserId == userId)).ToListAsync().ConfigureAwait(false);
        var categoryDtos = categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, IsSystemCategory = c.IsSystemCategory }).ToList();
        return (categoryDtos);
    }

    public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int userId, int categoryId, UpdateCategoryDto request)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && (c.IsSystemCategory || c.UserId == userId)).ConfigureAwait(false);
        if (category == null)
        {
            return new ApiResponse<CategoryDto> { Success = false, Message = "Category not found" };
        }

        //check if category is system category
        if (category.IsSystemCategory)
        {
            return new ApiResponse<CategoryDto> { Success = false, Message = "Can not update pre-defined categories" };
        }

        // Check if new name is duplicate (case-insensitive, excluding current category)
        var isDuplicate = await _context.Categories.AnyAsync(c => (c.IsSystemCategory || c.UserId == userId) && c.Name.ToLower().Trim() == request.Name.ToLower().Trim() && c.Id != categoryId).ConfigureAwait(false);
        if (isDuplicate)
        {
            return new ApiResponse<CategoryDto> { Success = false, Message = "Category with this name already exists" };
        }

        category.Name = request.Name;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name, IsSystemCategory = category.IsSystemCategory };
        var response = new ApiResponse<CategoryDto> { Success = true, Message = "Category updated successfully", Data = categoryDto };
        return (response);
    }

    public async Task<ApiResponse> DeleteCategoryAsync(int userId, int categoryId)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && (c.IsSystemCategory || c.UserId == userId)).ConfigureAwait(false);
        if (category == null)
        {
            return new ApiResponse { Success = false, Message = "Category not found" };
        }
        //check if category is not system category
        if (category.IsSystemCategory)
        {
            return new ApiResponse { Success = false, Message = "Can not delete pre-defined categories" };
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return new ApiResponse { Success = true, Message = "Category deleted successfully" };
    }
}
