using Finance.Api.Application.DTOs.Expenses;

namespace Finance.Api.Application.Interfaces;

public interface IAiExpenseService
{
    Task<FilterExpenseDto> ExtractFiltersAsync(
        string userId,
        string question,
        CancellationToken cancellationToken);
}
