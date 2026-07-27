using Finance.Api.Application.Interfaces;
using Finance.Api.Domain.Entities;
using Finance.Api.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Infrastructure.Repositories;

internal sealed class CategoryRepository(
    FinanceDbContext context)
    : ICategoryRepository
{
    public async Task<Category?> GetCategoryByIdAsync(
        int categoryId,
        int userId,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .FirstOrDefaultAsync(
                c => c.Id == categoryId &&
                     (c.IsSystemCategory || c.UserId == userId),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Category?> GetCategoryByNameAsync(
        string categoryName,
        int userId,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .FirstOrDefaultAsync(
                c => c.Name.ToUpper().Trim() == categoryName.ToUpperInvariant().Trim() && (c.IsSystemCategory || c.UserId == userId), cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> CategoryExistsAsync(
        string categoryName,
        int userId,
        int? excludeCategoryId,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .AnyAsync(
                c =>
                    (c.IsSystemCategory || c.UserId == userId) &&
                    c.Name.ToUpperInvariant().Trim() ==
                    categoryName.ToUpperInvariant().Trim() &&
                    (!excludeCategoryId.HasValue ||
                     c.Id != excludeCategoryId.Value),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<List<Category>> GetCategoriesAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(
                c => c.IsSystemCategory ||
                     c.UserId == userId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddCategoryAsync(
        Category category,
        CancellationToken cancellationToken)
    {
        _ = await context.Categories
            .AddAsync(
                category,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public Task DeleteCategoryAsync(
        Category category)
    {
        _ = context.Categories.Remove(category);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        _ = await context.SaveChangesAsync(
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<int> GetNextCategoryIdAsync(
    CancellationToken cancellationToken)
    {
        var categoryIds =
            await context.Categories
                .AsNoTracking()
                .Select(c => c.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        if (categoryIds.Count == 0)
        {
            return 1;
        }

        return categoryIds.Max() + 1;
    }
}
