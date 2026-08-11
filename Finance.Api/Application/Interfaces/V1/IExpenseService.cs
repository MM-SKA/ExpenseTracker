using Finance.Api.Application.DTOs.Common;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.V1.Expenses;

namespace Finance.Api.Application.Interfaces.V1;

public interface IExpenseServiceV1<T>
{
    Task<ApiResponse<ExpenseDtoV1>> CreateExpenseAsync(
        string userId,
        CreateExpenseDto request,
        CancellationToken cancellationToken);

    Task<ApiResponse<ExpenseDtoV1>> UpdateExpenseAsync(
        string userId,
        string id,
        UpdateExpenseDto request,
        CancellationToken cancellationToken);

    Task<ApiResponse> DeleteExpenseAsync(
        string userId,
        string id,
        CancellationToken cancellationToken);

    Task<List<ExpenseDtoV1>> GetExpenseAsync(
        string userId,
        CancellationToken cancellationToken);

    // Task<FilterResponseDto> FilterExpensesWithAnalyticsAsync(
    //     string userId,
    //     FilterExpenseDto filters,
    //     CancellationToken cancellationToken);

    Task<PaginationResponseDto<ExpenseDtoV1>> GetPaginatedExpensesAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<List<ExpenseDtoV1>> GlobalSearchAsync(
        string userId,
        SearchRequestDto request,
        CancellationToken cancellationToken);

    Task<ApiResponse<ExpenseDtoV1>> GetExpenseByIdAsync(
        string userId,
        string id,
        CancellationToken cancellationToken);

    Task<FilteredPagedExpenseResponseDto> FilterPagedExpensesAsync(
        string userId,
        FilterExpenseDto filters,
        CancellationToken cancellationToken);
}
