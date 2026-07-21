using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Domain.Entities;
using Finance.Api.Application.DTOs.V1.Expenses;

namespace Finance.Api.Application.Interfaces.V1;

public interface IExpenseServiceV1<T>
{
    Task<ApiResponse<ExpenseDtoV1>> CreateExpenseAsync(int userId, CreateExpenseDto request);
    Task<ApiResponse<ExpenseDtoV1>> UpdateExpenseAsync(int userId, int id, UpdateExpenseDto request);
    Task<ApiResponse> DeleteExpenseAsync(int userId, int id);

    Task<List<ExpenseDtoV1>> GetExpenseAsync(int userId);

    // Task<FilteredAnalyticsDto> CalculateAnalyticsAsync(List<Expense> filteredExpenses);

    Task<FilterResponseDto> FilterExpensesWithAnalyticsAsync(int userId, FilterExpenseDto filters);

    Task<PaginationResponseDto<ExpenseDtoV1>> GetPaginatedExpensesAsync(int userId, int pageNumber, int pageSize);

    Task<List<ExpenseDtoV1>> GlobalSearchAsync(int userId, SearchRequestDto request);
}
