using Finance.Api.Application.Interfaces;
using Finance.Api.Infrastructure.Data;
using Finance.Api.Application.DTOs.Category;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Application.Logs;
using Finance.Api.Domain.Entities;
using Finance.Api.Application.Constants;

using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Application.Services;

internal sealed class CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger) : ICategoryService
{

    public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(string userId, CreateCategoryDto request, CancellationToken cancellationToken)
    {
        // var userId = User.GetUserId();

        // Check if category with same name already exists for this user
        ArgumentNullException.ThrowIfNull(request);
        var existingCategory = await categoryRepository.GetCategoryByNameAsync(request.Name, userId, cancellationToken).ConfigureAwait(false);

        if (existingCategory != null)
        {
            CategoryServiceLogs.DuplicateCategory(logger);
            return new ApiResponse<CategoryDto> { Success = false, Message = "Category with this name already exists" };
        }

        var category = new Category
        {
            Name = request.Name.Trim(),
            UserId = userId,
            IsSystemCategory = false
        };

        await categoryRepository.AddCategoryAsync(category, cancellationToken).ConfigureAwait(false);
        await categoryRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        CategoryServiceLogs.CategoryCreated(logger, userId);
        var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name, IsSystemCategory = category.IsSystemCategory };
        var response = new ApiResponse<CategoryDto> { Success = true, Message = "Category created successfully", Data = categoryDto };
        return response;
    }

    public async Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(string userId, string id, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id, userId, cancellationToken).ConfigureAwait(false);
        if (category == null)
        {
            return new ApiResponse<CategoryDto> { Success = false, Message = "Category not found", ErrorCode = ErrorCodes.CategoryNotFound };
        }
        return new ApiResponse<CategoryDto> { Success = true, Message = "Category Found", Data = new CategoryDto {Id = category.Id, Name = category.Name }};
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync(string userId, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetCategoriesAsync(userId, cancellationToken).ConfigureAwait(false);
        var categoryDtos = categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, IsSystemCategory = c.IsSystemCategory }).ToList();
        return (categoryDtos);
    }

    public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(string userId, string id, UpdateCategoryDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var category = await categoryRepository.GetCategoryByIdAsync(id, userId, cancellationToken).ConfigureAwait(false);
        if (category == null)
        {
            return new ApiResponse<CategoryDto> { Success = false, Message = "Category not found", ErrorCode = ErrorCodes.CategoryNotFound };
        }

        //check if category is system category
        if (category.IsSystemCategory)
        {
            return new ApiResponse<CategoryDto> { Success = false, Message = "Can not update pre-defined categories", ErrorCode = ErrorCodes.SystemCategoryUpdate };
        }

        // Check if new name is duplicate (case-insensitive, excluding current category)
        var isDuplicate = await categoryRepository.CategoryExistsAsync(request.Name, userId, id, cancellationToken).ConfigureAwait(false);
        if (isDuplicate)
        {
            return new ApiResponse<CategoryDto> { Success = false, Message = "Category with this name already exists", ErrorCode = ErrorCodes.DuplicateCategory };
        }

        category.Name = request.Name;
        await categoryRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name, IsSystemCategory = category.IsSystemCategory };
        var response = new ApiResponse<CategoryDto> { Success = true, Message = "Category updated successfully", Data = categoryDto };
        return (response);
    }

    public async Task<ApiResponse> DeleteCategoryAsync(string userId, string id, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id, userId, cancellationToken).ConfigureAwait(false);
        if (category == null)
        {
            return new ApiResponse { Success = false, Message = "Category not found" };
        }
        //check if category is not system category
        if (category.IsSystemCategory)
        {
            return new ApiResponse { Success = false, Message = "Can not delete pre-defined categories", ErrorCode = ErrorCodes.SystemCategoryUpdate };
        }
        await categoryRepository.DeleteCategoryAsync(category).ConfigureAwait(false);
        await categoryRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new ApiResponse { Success = true, Message = "Category deleted successfully" };
    }
}
