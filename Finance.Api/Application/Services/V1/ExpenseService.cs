using Finance.Api.Application.Constants;
using Finance.Api.Application.DTOs.Category;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.V1.Expenses;
using Finance.Api.Application.Interfaces;
using Finance.Api.Application.Interfaces.V1;
using Finance.Api.Domain.Entities;

using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Application.Services.V1;

internal sealed class ExpenseServiceV1(
    IExpenseRepository expenseRepository)
    : IExpenseServiceV1<ExpenseDtoV1>
{
    public async Task<ApiResponse<ExpenseDtoV1>> CreateExpenseAsync(
        string userId,
        CreateExpenseDto request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category =
            await expenseRepository.GetCategoryByIdAsync(
                    request.CategoryId,
                    userId,
                    cancellationToken)
                .ConfigureAwait(false);

        if (category == null)
        {
            return new ApiResponse<ExpenseDtoV1>
            {
                Success = false,
                Message = "Invalid category",
                ErrorCode = ErrorCodes.InvalidCategory
            };
        }

        var expense = new Expense
        {
            CategoryId = request.CategoryId,
            CategoryName = category.Name,
            Notes = request.Notes,
            Amount = request.Amount,
            ExpenseDate = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
            UserId = userId,
            Location = request.Location
        };

        await expenseRepository
            .AddExpenseAsync(
                expense,
                cancellationToken)
            .ConfigureAwait(false);

        await expenseRepository
            .SaveChangesAsync(
                cancellationToken)
            .ConfigureAwait(false);

        var expenseDto = new ExpenseDtoV1
        {
            Id = expense.Id,
            Description = expense.Notes ?? string.Empty,
            Amount = expense.Amount,
            Date = expense.ExpenseDate,
            CategoryId = expense.CategoryId
        };

        return new ApiResponse<ExpenseDtoV1>
        {
            Success = true,
            Message = "Expense created successfully",
            Data = expenseDto
        };
    }

    public async Task<ApiResponse<ExpenseDtoV1>> UpdateExpenseAsync(
        string userId,
        string id,
        UpdateExpenseDto request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var expense =
            await expenseRepository.GetExpenseByIdAsync(
                    id,
                    userId,
                    cancellationToken)
                .ConfigureAwait(false);

        if (expense == null)
        {
            return new ApiResponse<ExpenseDtoV1>
            {
                Success = false,
                Message = "Expense not found",
                ErrorCode = ErrorCodes.ExpenseNotFound
            };
        }

        var category =
            await expenseRepository.GetCategoryByIdAsync(
                    request.CategoryId,
                    userId,
                    cancellationToken)
                .ConfigureAwait(false);

        if (category == null)
        {
            return new ApiResponse<ExpenseDtoV1>
            {
                Success = false,
                Message = "Invalid category",
                ErrorCode = ErrorCodes.InvalidCategory
            };
        }

        expense.CategoryId = request.CategoryId;
        expense.Notes = request.Notes;
        expense.Amount = request.Amount;
        expense.ExpenseDate = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc);
        expense.Location = request.Location;

        await expenseRepository
            .SaveChangesAsync(
                cancellationToken)
            .ConfigureAwait(false);

        var expenseDto = new ExpenseDtoV1
        {
            Id = expense.Id,
            Description = expense.Notes ?? string.Empty,
            Amount = expense.Amount,
            Date = expense.ExpenseDate,
            CategoryId = expense.CategoryId,
            Location = expense.Location
        };

        return new ApiResponse<ExpenseDtoV1>
        {
            Success = true,
            Message = "Expense updated successfully",
            Data = expenseDto
        };
    }

    public async Task<ApiResponse> DeleteExpenseAsync(
        string userId,
        string id,
        CancellationToken cancellationToken)
    {
        var expense =
            await expenseRepository.GetExpenseByIdAsync(
                    id,
                    userId,
                    cancellationToken)
                .ConfigureAwait(false);

        if (expense == null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = "Expense not found",
                ErrorCode = ErrorCodes.ExpenseNotFound
            };
        }

        await expenseRepository
            .DeleteExpenseAsync(expense)
            .ConfigureAwait(false);

        await expenseRepository
            .SaveChangesAsync(
                cancellationToken)
            .ConfigureAwait(false);

        return new ApiResponse
        {
            Success = true,
            Message = "Expense deleted successfully"
        };
    }

    public async Task<List<ExpenseDtoV1>> GetExpenseAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var expenses =
            await expenseRepository
                .GetExpenseQuery(userId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        return expenses
            .Select(e => new ExpenseDtoV1
            {
                Id = e.Id,
                CategoryName = e.CategoryName,
                Description = e.Notes ?? string.Empty,
                Amount = e.Amount,
                Date = e.ExpenseDate,
                CategoryId = e.CategoryId,
                Location = e.Location
            })
            .ToList();
    }

    public async Task<FilterResponseDto> FilterExpensesWithAnalyticsAsync(
        string userId,
        FilterExpenseDto filters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filters);

        var query =
            BuildFilteredQuery(
                userId,
                filters);

        var filteredExpenses =
            await query
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        var response = new FilterResponseDto
        {
            Expenses =
            [
                .. filteredExpenses.Select(e => new ExpenseDtoV1
                {
                    Id = e.Id,
                    Description = e.Notes ?? string.Empty,
                    Amount = e.Amount,
                    Date = e.ExpenseDate,
                    CategoryId = e.CategoryId,
                    Location = e.Location
                })
            ]
        };

        if (filters.includeAnalytics)
        {
            response.Analytics =
                CalculateAnalytics(filteredExpenses);
        }

        return response;
    }

    private IQueryable<Expense> BuildFilteredQuery(
        string userId,
        FilterExpenseDto filters)
    {
        var query =
            expenseRepository.GetExpenseQuery(userId);

        if (!string.IsNullOrWhiteSpace(filters.categoryId))
        {
            query = query.Where(
                e => e.CategoryId == filters.categoryId);
        }

        if (filters.startDate.HasValue)
        {
            query = query.Where(
                e => e.ExpenseDate >= filters.startDate.Value.Date);
        }

        if (filters.endDate.HasValue)
        {
            query = query.Where(
                e => e.ExpenseDate <= filters.endDate.Value.Date);
        }

        if (filters.minAmount.HasValue)
        {
            query = query.Where(
                e => e.Amount >= filters.minAmount.Value);
        }

        if (filters.maxAmount.HasValue)
        {
            query = query.Where(
                e => e.Amount <= filters.maxAmount.Value);
        }

        if (filters.month.HasValue)
        {
            query = query.Where(
                e => e.ExpenseDate.Month == filters.month.Value);
        }

        if (filters.year.HasValue)
        {
            query = query.Where(
                e => e.ExpenseDate.Year == filters.year.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.notes))
        {
            query = query.Where(
                e => e.Notes != null &&
                     e.Notes.Contains(filters.notes));
        }

        if (!string.IsNullOrWhiteSpace(filters.Location))
        {
            query = query.Where(
                e => e.Location != null &&
                e.Location.Contains(filters.Location));
        }

        return query.OrderByDescending(e => e.ExpenseDate);
    }

    private static FilteredAnalyticsDto CalculateAnalytics(
        List<Expense> expenses)
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
        var averageAmount =
            count > 0
                ? totalSpent / count
                : 0;

        var minAmount = expenses.Min(e => e.Amount);
        var maxAmount = expenses.Max(e => e.Amount);

        var firstDate =
            expenses.Min(e => e.ExpenseDate).Date;

        var lastDate =
            expenses.Max(e => e.ExpenseDate).Date;

        var daysWithExpenses =
            expenses
                .Select(e => e.ExpenseDate.Date)
                .Distinct()
                .Count();

        var categories =
            expenses
                .GroupBy(e => new
                {
                    e.CategoryId,
                    e.CategoryName
                })
                .Select(g => new CategorySpendDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    Amount = g.Sum(e => e.Amount),
                    TransactionCount = g.Count(),
                    Percentage =
                        totalSpent > 0
                            ? (double)(g.Sum(e => e.Amount) / totalSpent * 100m)
                            : 0
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

    public async Task<PaginationResponseDto<ExpenseDtoV1>>
        GetPaginatedExpensesAsync(
            string userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
    {
        var query =
            expenseRepository.GetExpenseQuery(userId);

        var totalRecords =
            await EntityFrameworkQueryableExtensions
                .CountAsync(query, cancellationToken)
                .ConfigureAwait(false);

        var totalPages =
            (int)Math.Ceiling(
                totalRecords / (double)pageSize);

        totalPages =
            Math.Max(totalPages, 1);

        if (pageNumber > totalPages)
        {
            pageNumber = totalPages;
        }

        var expenses =
            await query
                .OrderByDescending(e => e.ExpenseDate)
                .ThenByDescending(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        var expenseDtos =
            expenses
                .Select(e => new ExpenseDtoV1
                {
                    Id = e.Id,
                    Description = e.Notes ?? string.Empty,
                    Amount = e.Amount,
                    Date = e.ExpenseDate,
                    CategoryId = e.CategoryId,
                    Location = e.Location
                })
                .ToList();

        return new PaginationResponseDto<ExpenseDtoV1>
        {
            Items = expenseDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }

    public async Task<List<ExpenseDtoV1>> GlobalSearchAsync(
        string userId,
        SearchRequestDto request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var expenses =
            await expenseRepository
                .GetExpenseQuery(userId)
                .Where(e =>
                    e.CategoryName.Contains(request.Query) ||
                    (
                        e.Notes != null &&
                        e.Notes.Contains(request.Query)
                    ) ||
                    (
                        e.Location != null &&
                        e.Location.Contains(request.Query)
                    ))
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        return
        [
            .. expenses.Select(e => new ExpenseDtoV1
            {
                Id = e.Id,
                Description = e.Notes ?? string.Empty,
                Amount = e.Amount,
                Date = e.ExpenseDate,
                CategoryId = e.CategoryId,
                Location = e.Location
            })
        ];
    }

    public async Task<ApiResponse<ExpenseDtoV1>> GetExpenseByIdAsync(string userId, string id, CancellationToken cancellationToken)
    {
        var expense = await expenseRepository.GetExpenseByIdAsync(id, userId, cancellationToken).ConfigureAwait(false);
        if (expense == null)
        {
            return new ApiResponse<ExpenseDtoV1>
            {
                Success = false,
                Message = "Expense not found",
                ErrorCode = ErrorCodes.ExpenseNotFound
            };
        }

        var expenseDto = new ExpenseDtoV1
        {
            Id = expense.Id,
            Description = expense.Notes ?? string.Empty,
            Amount = expense.Amount,
            Date = expense.ExpenseDate,
            CategoryId = expense.CategoryId,
            Location = expense.Location
        };

        return new ApiResponse<ExpenseDtoV1>
        {
            Success = true,
            Message = "Expense fetched successfully",
            Data = expenseDto
        };
    }
}
