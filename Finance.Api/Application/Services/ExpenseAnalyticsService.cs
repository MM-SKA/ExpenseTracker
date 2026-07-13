using Finance.Api.Data;
using Finance.Api.DTOs.Expenses;
using Finance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Application.Services;

public class ExpenseAnalyticsService : IExpenseAnalyticsService
{
    private readonly FinanceDbContext _context;

    public ExpenseAnalyticsService(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<FilterResponseDto> FilterExpensesWithAnalyticsAsync(int userId, FilterExpenseDto filters)
    {
        var query = BuildFilteredQuery(userId, filters);
        var filteredExpenses = await query.ToListAsync();

        var response = new FilterResponseDto
        {
            Expenses = filteredExpenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                Description = e.Notes??string.Empty,
                Amount = e.Amount,
                Date = e.ExpenseDate,
                CategoryId = e.CategoryId
            }).ToList()
        };

        if (filters.includeAnalytics)
        {
            response.Analytics = CalculateAnalytics(filteredExpenses);
        }

        return response;
    }

    public Task<FilteredAnalyticsDto> CalculateAnalyticsAsync(List<Expense> filteredExpenses)
    {
        return Task.FromResult(CalculateAnalytics(filteredExpenses));
    }

    private IQueryable<Expense> BuildFilteredQuery(int userId, FilterExpenseDto filters)
    {
        var query = _context.Expenses.AsNoTracking().Include(e => e.Category).Where(e => e.UserId == userId).AsQueryable();

        if (filters.categoryId.HasValue)
            query = query.Where(e => e.CategoryId == filters.categoryId.Value );

        if (filters.startDate.HasValue)
            query = query.Where(e => e.ExpenseDate >= filters.startDate.Value.Date);

        if (filters.endDate.HasValue)
            query = query.Where(e => e.ExpenseDate <= filters.endDate.Value.Date);

        if (filters.minAmount.HasValue)
            query = query.Where(e => e.Amount >= filters.minAmount.Value);

        if (filters.maxAmount.HasValue)
            query = query.Where(e => e.Amount <= filters.maxAmount.Value);

        if (filters.month.HasValue)
            query = query.Where(e => e.ExpenseDate.Month == filters.month.Value);

        if (filters.year.HasValue)
            query = query.Where(e => e.ExpenseDate.Year == filters.year.Value);

        if (!string.IsNullOrEmpty(filters.notes))
            query = query.Where(e => e.Notes != null && e.Notes.Contains(filters.notes));

        return query;
    }

    private FilteredAnalyticsDto CalculateAnalytics(List<Expense> expenses)
    {
        if (!expenses.Any())
        {
            return new FilteredAnalyticsDto
            {
                Summary = new SummaryDto(),
                ByCategory = new List<CategorySpendDto>(),
                DateDistribution = new DateDistributionDto()
            };
        }

        var totalSpent = expenses.Sum(e => e.Amount);
        var count = expenses.Count;
        var averageAmount = count > 0 ? totalSpent / count : 0;
        var minAmount = expenses.Min(e => e.Amount);
        var maxAmount = expenses.Max(e => e.Amount);

        var firstDate = expenses.Min(e => e.ExpenseDate).Date;
        var lastDate = expenses.Max(e => e.ExpenseDate).Date;
        var daysWithExpenses = expenses.Select(e => e.ExpenseDate.Date).Distinct().Count();

        var categories = expenses
            .GroupBy(e => new { e.CategoryId, e.Category.Name })
            .Select(g => new CategorySpendDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Amount = g.Sum(e => e.Amount),
                TransactionCount = g.Count(),
                Percentage = totalSpent > 0 ? (double)(g.Sum(e => e.Amount) / totalSpent * 100m) : 0
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        return new FilteredAnalyticsDto
        {
            Summary = new SummaryDto
            {
                TotalSpent = totalSpent,
                Count = count,
                AverageAmount = averageAmount,
                MinAmount = minAmount,
                MaxAmount = maxAmount
            },
            ByCategory = categories,
            DateDistribution = new DateDistributionDto
            {
                StartDate = firstDate,
                EndDate = lastDate,
                DaysWithExpenses = daysWithExpenses
            }
        };
    }
}
