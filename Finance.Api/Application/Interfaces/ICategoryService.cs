using Finance.Api.Application.DTOs.Category;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;

namespace Finance.Api.Application.Interfaces;

public interface ICategoryService
{
    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(int userId, CreateCategoryDto request, CancellationToken cancellationToken);

    Task<List<CategoryDto>> GetCategoriesAsync(int userId, CancellationToken cancellationToken);

    Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int userId, int id, UpdateCategoryDto request, CancellationToken cancellationToken);

    Task<ApiResponse> DeleteCategoryAsync(int userId, int id, CancellationToken cancellationToken);

}
