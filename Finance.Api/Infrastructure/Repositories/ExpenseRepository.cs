using Finance.Api.Application.Interfaces;
using Finance.Api.Domain.Entities;
using Finance.Api.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Infrastructure.Repositories;

internal sealed class ExpenseRepository(
    FinanceDbContext context)
    : IExpenseRepository
{
    public IQueryable<Expense> GetExpenseQuery(
        int userId)
    {
        return context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.UserId == userId);
    }

    public async Task<Expense?> GetExpenseByIdAsync(
        int expenseId,
        int userId,
        CancellationToken cancellationToken)
    {
        return await context.Expenses
            .FirstOrDefaultAsync(
                e => e.Id == expenseId &&
                     e.UserId == userId,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Category?> GetCategoryByIdAsync(
        int categoryId,
        int userId,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .FirstOrDefaultAsync(
                c => c.Id == categoryId &&
                     (
                         c.IsSystemCategory ||
                         c.UserId == userId
                     ),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public Task AddExpenseAsync(
        Expense expense,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(expense);

        context.Expenses.Add(expense);

        return Task.CompletedTask;
    }

    public Task DeleteExpenseAsync(
        Expense expense)
    {
        ArgumentNullException.ThrowIfNull(expense);

        context.Expenses.Remove(expense);

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
