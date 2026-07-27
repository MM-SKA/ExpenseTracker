using Finance.Api.Domain.Entities;

namespace Finance.Api.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetCategoryByIdAsync(
        int categoryId,
        int userId,
        CancellationToken cancellationToken);

    Task<Category?> GetCategoryByNameAsync(
        string categoryName,
        int userId,
        CancellationToken cancellationToken);

    Task<bool> CategoryExistsAsync(
        string categoryName,
        int userId,
        int? excludeCategoryId,
        CancellationToken cancellationToken);

    Task<List<Category>> GetCategoriesAsync(
        int userId,
        CancellationToken cancellationToken);

    Task AddCategoryAsync(
        Category category,
        CancellationToken cancellationToken);

    Task DeleteCategoryAsync(
        Category category);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}
