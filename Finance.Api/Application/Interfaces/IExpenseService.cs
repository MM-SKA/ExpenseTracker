using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Domain.Entities;

namespace Finance.Api.Application.Interfaces;

public interface IExpenseService<T>
{
    Task<ApiResponse<ExpenseDto>> CreateExpense(int userId, CreateExpenseDto request);
    Task<ApiResponse<ExpenseDto>> UpdateExpense(int userId, int id, UpdateExpenseDto request);
    Task<ApiResponse> DeleteExpense(int userId, int id);

    Task<List<ExpenseDto>> GetExpense(int userId);

    // Task<FilteredAnalyticsDto> CalculateAnalyticsAsync(List<Expense> filteredExpenses);

    Task<FilterResponseDto> FilterExpensesWithAnalyticsAsync(int userId, FilterExpenseDto filters);

    Task<PaginationResponseDto<ExpenseDto>> GetPaginatedExpensesAsync(int userId, int pageNumber, int pageSize);

    Task<List<ExpenseDto>> GlobalSearchAsync(int userId, SearchRequestDto request);
}
