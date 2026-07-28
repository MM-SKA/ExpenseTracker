using Finance.Api.Application.DTOs.Category;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;

namespace Finance.Api.Application.Interfaces;

public interface ICategoryService
{
    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(string userId, CreateCategoryDto request, CancellationToken cancellationToken);

    Task<List<CategoryDto>> GetCategoriesAsync(string userId, CancellationToken cancellationToken);

    Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(string userId, string id, UpdateCategoryDto request, CancellationToken cancellationToken);

    Task<ApiResponse> DeleteCategoryAsync(string userId, string id, CancellationToken cancellationToken);

}
