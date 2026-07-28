using Finance.Api.Domain.Entities;

namespace Finance.Api.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetCategoryByIdAsync(
        string categoryId,
        string userId,
        CancellationToken cancellationToken);

    Task<Category?> GetCategoryByNameAsync(
        string categoryName,
        string userId,
        CancellationToken cancellationToken);

    Task<bool> CategoryExistsAsync(
        string categoryName,
        string userId,
        string? excludeCategoryId,
        CancellationToken cancellationToken);

    Task<List<Category>> GetCategoriesAsync(
        string userId,
        CancellationToken cancellationToken);

    Task AddCategoryAsync(
        Category category,
        CancellationToken cancellationToken);

    Task DeleteCategoryAsync(
        Category category);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}
