using Finance.Api.Application.Interfaces;
using Finance.Api.Data;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Models;
using Microsoft.EntityFrameworkCore;

public class ExpenseService : IExpenseService
{

    private readonly FinanceDbContext _context;
    private readonly IExpenseAnalyticsService _analyticsService;

    public ExpenseService(FinanceDbContext context, IExpenseAnalyticsService analyticsService)
    {
        _context = context;
        _analyticsService = analyticsService;
    }
    public async Task<ApiResponse<ExpenseDto>> CreateExpense(int userId , CreateExpenseDto request)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.IsSystemCategory || c.UserId == userId));
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
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
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
    public async Task<ApiResponse<ExpenseDto>> UpdateExpense(int userId , int id, UpdateExpenseDto request)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense == null)
        {
            return new ApiResponse<ExpenseDto> { Success = false, Message = "Expense not found" };
        }

        // Verify category belongs to user
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId);
        if (category == null)
        {
            return new ApiResponse<ExpenseDto> { Success = false, Message = "Invalid category" };
        }

        expense.CategoryId = request.CategoryId;
        expense.Notes = request.Notes;
        expense.Amount = request.Amount;
        expense.ExpenseDate = request.Date;

        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();

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

    public async Task<ApiResponse> DeleteExpense(int userId , int id)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense == null)
        {
            return new ApiResponse { Success = false, Message = "Expense not found" };
        }
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return new ApiResponse{Success=true , Message = "Expense Deleted successfully"};
    }

    public async Task<List<ExpenseDto>> GetExpense(int userId)
    {
        var expenses = await _context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.UserId == userId)
            .ToListAsync();
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
}