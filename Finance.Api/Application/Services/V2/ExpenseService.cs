using Finance.Api.Application.Interfaces.V2;
using Finance.Api.Infrastructure.Data;
using Finance.Api.Application.DTOs.V2.Expenses;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Finance.Api.Application.DTOs.Category;

namespace Finance.Api.Application.Services.V2;

internal sealed class ExpenseServiceV2(FinanceDbContext context) : IExpenseServiceV2<ExpenseDtoV2>
{
    private readonly FinanceDbContext _context = context;

    public async Task<List<ExpenseDtoV2>> GetExpensesAsync(int userId)
    {
        var expenses = await _context.Expenses.AsNoTracking().Include(e => e.Category).Where(e => e.UserId == userId).ToListAsync().ConfigureAwait(false);
        var expenseDtos = expenses.Select(e => new ExpenseDtoV2
        {
            Id = e.Id,
            Description = e.Notes ?? string.Empty,
            Amount = e.Amount,
            Date = e.ExpenseDate,
            CategoryId = e.CategoryId,
            Currency = "INR"
        }).ToList();
        return expenseDtos;
    }
}
