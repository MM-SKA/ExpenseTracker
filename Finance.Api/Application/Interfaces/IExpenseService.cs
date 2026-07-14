using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;

namespace Finance.Api.Application.Interfaces;

public interface IExpenseService
{
    Task<ApiResponse<ExpenseDto>> CreateExpense(int userId , CreateExpenseDto request);
    Task<ApiResponse<ExpenseDto>> UpdateExpense(int userId , int id, UpdateExpenseDto request);
    Task<ApiResponse> DeleteExpense(int userId , int id);

    Task<List<ExpenseDto>> GetExpense(int userId);
    // Task<ApiResponse<FilterResponseDto>> FilterExpense(int userId , FilterExpenseDto request);
}