using Finance.Api.Application.DTOs.Common;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.V1.Expenses;

namespace Finance.Api.Application.Interfaces.V1;

public interface IExpenseServiceV1<T>
{
    Task<ApiResponse<ExpenseDtoV1>> CreateExpenseAsync(
        int userId,
        CreateExpenseDto request,
        CancellationToken cancellationToken);

    Task<ApiResponse<ExpenseDtoV1>> UpdateExpenseAsync(
        int userId,
        int id,
        UpdateExpenseDto request,
        CancellationToken cancellationToken);

    Task<ApiResponse> DeleteExpenseAsync(
        int userId,
        int id,
        CancellationToken cancellationToken);

    Task<List<ExpenseDtoV1>> GetExpenseAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<FilterResponseDto> FilterExpensesWithAnalyticsAsync(
        int userId,
        FilterExpenseDto filters,
        CancellationToken cancellationToken);

    Task<PaginationResponseDto<ExpenseDtoV1>> GetPaginatedExpensesAsync(
        int userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<List<ExpenseDtoV1>> GlobalSearchAsync(
        int userId,
        SearchRequestDto request,
        CancellationToken cancellationToken);
}
