using Finance.Api.Domain.Entities;

namespace Finance.Api.Application.Interfaces;

public interface IExpenseRepository
{
    IQueryable<Expense> GetExpenseQuery(int userId);

    Task<Expense?> GetExpenseByIdAsync(
        int expenseId,
        int userId,
        CancellationToken cancellationToken);

    Task<Category?> GetCategoryByIdAsync(
        int categoryId,
        int userId,
        CancellationToken cancellationToken);

    Task AddExpenseAsync(
        Expense expense,
        CancellationToken cancellationToken);

    Task DeleteExpenseAsync(
        Expense expense);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}
