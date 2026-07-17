using Finance.Api.Application.Interfaces;
using Finance.Api.Infrastructure.Data;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Finance.Api.Application.DTOs.Category;

namespace Finance.Api.Application.Services;

public class ExpenseService(FinanceDbContext context) : IExpenseService<ExpenseDto>
{
    private readonly FinanceDbContext _context = context;

    public async Task<ApiResponse<ExpenseDto>> CreateExpense(int userId, CreateExpenseDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.IsSystemCategory || c.UserId == userId)).ConfigureAwait(false);
        if (category == null)
        {
            return new ApiResponse<ExpenseDto> { Success = false, Message = "Invalid category" };
        }
        var expense = new Expense
        {
            CategoryId = request.CategoryId,
            Notes = request.Notes,
            Amount = request.Amount,
            ExpenseDate = request.Date,
            UserId = userId
        };
        _ = _context.Expenses.Add(expense);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        var expenseDto = new ExpenseDto
        {
            Id = expense.Id,
            Description = expense.Notes,
            Amount = expense.Amount,
            Date = expense.ExpenseDate,
            CategoryId = expense.CategoryId
        };
        var response = new ApiResponse<ExpenseDto> { Success = true, Message = "Expense created successfully", Data = expenseDto };
        return response;
    }
    public async Task<ApiResponse<ExpenseDto>> UpdateExpense(int userId, int id, UpdateExpenseDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId).ConfigureAwait(false);
        if (expense == null)
        {
            return new ApiResponse<ExpenseDto> { Success = false, Message = "Expense not found" };
        }

        // Verify category belongs to user
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId).ConfigureAwait(false);
        if (category == null)
        {
            return new ApiResponse<ExpenseDto> { Success = false, Message = "Invalid category" };
        }

        expense.CategoryId = request.CategoryId;
        expense.Notes = request.Notes;
        expense.Amount = request.Amount;
        expense.ExpenseDate = request.Date;

        _ = _context.Expenses.Update(expense);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);

        var expenseDto = new ExpenseDto
        {
            Id = expense.Id,
            Description = expense.Notes,
            Amount = expense.Amount,
            Date = expense.ExpenseDate,
            CategoryId = expense.CategoryId
        };
        var response = new ApiResponse<ExpenseDto> { Success = true, Message = "Expense updated successfully", Data = expenseDto };
        return response;
    }

    public async Task<ApiResponse> DeleteExpense(int userId, int id)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId).ConfigureAwait(false);
        if (expense == null)
        {
            return new ApiResponse { Success = false, Message = "Expense not found" };
        }
        _ = _context.Expenses.Remove(expense);
        _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        return new ApiResponse { Success = true, Message = "Expense Deleted successfully" };
    }

    public async Task<List<ExpenseDto>> GetExpense(int userId)
    {
        var expenses = await _context.Expenses.AsNoTracking().Include(e => e.Category).Where(e => e.UserId == userId).ToListAsync().ConfigureAwait(false);
        var expenseDtos = expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            Description = e.Notes ?? string.Empty,
            Amount = e.Amount,
            Date = e.ExpenseDate,
            CategoryId = e.CategoryId
        }).ToList();
        return expenseDtos;
    }

    // public async Task<ApiResponse<FilterResponseDto>> FilterExpense(int userId , FilterExpenseDto request)
    // {

    // }

    public async Task<FilterResponseDto> FilterExpensesWithAnalyticsAsync(int userId, FilterExpenseDto filters)
    {
        ArgumentNullException.ThrowIfNull(filters);
        var query = BuildFilteredQuery(userId, filters);
        var filteredExpenses = await query.ToListAsync().ConfigureAwait(false);

        var response = new FilterResponseDto
        {
            Expenses = filteredExpenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                Description = e.Notes ?? string.Empty,
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
        ArgumentNullException.ThrowIfNull(filteredExpenses);
        return Task.FromResult(CalculateAnalytics(filteredExpenses));
    }

    private IQueryable<Expense> BuildFilteredQuery(int userId, FilterExpenseDto filters)
    {
        var query = _context.Expenses.AsNoTracking().Include(e => e.Category).Where(e => e.UserId == userId).AsQueryable();

        if (filters.categoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == filters.categoryId.Value);
        }
        if (filters.startDate.HasValue)
        {
            query = query.Where(e => e.ExpenseDate >= filters.startDate.Value.Date);
        }
        if (filters.endDate.HasValue)
        {
            query = query.Where(e => e.ExpenseDate <= filters.endDate.Value.Date);
        }
        if (filters.minAmount.HasValue)
        {
            query = query.Where(e => e.Amount >= filters.minAmount.Value);
        }
        if (filters.maxAmount.HasValue)
        {
            query = query.Where(e => e.Amount <= filters.maxAmount.Value);
        }
        if (filters.month.HasValue)
        {
            query = query.Where(e => e.ExpenseDate.Month == filters.month.Value);
        }

        if (filters.year.HasValue)
        {
            query = query.Where(e => e.ExpenseDate.Year == filters.year.Value);

        }
        if (!string.IsNullOrEmpty(filters.notes))
        {
            query = query.Where(e => e.Notes != null && e.Notes.Contains(filters.notes));
        }

        return query;
    }

    private static FilteredAnalyticsDto CalculateAnalytics(List<Expense> expenses)
    {
        if (expenses.Count == 0)
        {
            return new FilteredAnalyticsDto
            {
                Summary = new SummaryDto(),
                ByCategory = [],
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

    public async Task<PaginationResponseDto<ExpenseDto>> GetPaginatedExpensesAsync(int userId, int pageNumber, int pageSize)
    {

        // if(pageSize<=0 || pageNumber <= 0)
        // {
        //     throw new Exception("PageNumber or PageSize can not be Negative nor Zero");
        // }

        var query = _context.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId);

        var totalRecords = await query.CountAsync().ConfigureAwait(false);

        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        totalPages = Math.Max(totalPages, 1);
        if (pageNumber > totalPages)
        {
            pageNumber = Math.Min(pageNumber, totalPages);
        }

        var expenses = await query.OrderByDescending(e => e.ExpenseDate).ThenByDescending(e => e.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

        var expenseDtos = expenses
            .Select(e => new ExpenseDto
            {
                Id = e.Id,
                Description = e.Notes ?? string.Empty,
                Amount = e.Amount,
                Date = e.ExpenseDate,
                CategoryId = e.CategoryId
            })
            .ToList();

        return new PaginationResponseDto<ExpenseDto>
        {
            Items = expenseDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages =
                (int)Math.Ceiling(
                    totalRecords /
                    (double)pageSize)
        };

    }

    public async Task<List<ExpenseDto>> GlobalSearchAsync(int userId, SearchRequestDto request)
    {
        var expenses = await _context.Expenses.AsNoTracking().Include(e => e.Category).Where(e => e.UserId == userId && (e.Category.Name.Contains(request.Query) || (e.Notes != null && e.Notes.Contains(request.Query)))).OrderByDescending(e => e.ExpenseDate).ToListAsync().ConfigureAwait(false);

        return expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            Description = e.Notes ?? string.Empty,
            Amount = e.Amount,
            Date = e.ExpenseDate,
            CategoryId = e.CategoryId
        }).ToList();
    }
}
