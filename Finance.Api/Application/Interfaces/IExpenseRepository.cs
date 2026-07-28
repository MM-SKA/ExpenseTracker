using Finance.Api.Domain.Entities;

namespace Finance.Api.Application.Interfaces;

public interface IExpenseRepository
{
    IQueryable<Expense> GetExpenseQuery(string userId);

    Task<Expense?> GetExpenseByIdAsync(
        string expenseId,
        string userId,
        CancellationToken cancellationToken);

    Task<Category?> GetCategoryByIdAsync(
        string categoryId,
        string userId,
        CancellationToken cancellationToken);

    Task AddExpenseAsync(
        Expense expense,
        CancellationToken cancellationToken);

    Task DeleteExpenseAsync(
        Expense expense);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}
