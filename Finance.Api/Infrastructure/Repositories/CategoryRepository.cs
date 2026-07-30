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
        string categoryId,
        string userId,
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
        string userId,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .FirstOrDefaultAsync(
                c => c.Name.ToUpper().Trim() == categoryName.ToUpperInvariant().Trim() && (c.IsSystemCategory || c.UserId == userId), cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> CategoryExistsAsync(
     string categoryName,
     string userId,
     string? excludeCategoryId,
     CancellationToken cancellationToken)
    {
        var normalizedCategoryName =
            categoryName.Trim();

        var categories =
            await context.Categories
                .Where(c =>
                    c.UserId == userId ||
                    c.IsSystemCategory)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        return categories.Any(c =>
            !string.Equals(
                c.Id,
                excludeCategoryId,
                StringComparison.OrdinalIgnoreCase)
            &&
            string.Equals(
                c.Name.Trim(),
                normalizedCategoryName,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Category>> GetCategoriesAsync(
        string userId,
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
}
