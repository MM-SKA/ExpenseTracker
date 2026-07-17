using Finance.Api.Application.DTOs.Categories;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;

namespace Finance.Api.Application.Interfaces;

public interface ICategoryService
{
    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(int userId, CreateCategoryDto request);

    Task<List<CategoryDto>> GetCategoriesAsync(int userId);

    Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int userId, int id, UpdateCategoryDto request);

    Task<ApiResponse> DeleteCategoryAsync(int userId, int id);

}
