using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Finance.Api.DTOs.Expenses;
using Finance.Api.DTOs.Common;
using Finance.Api.Models;
using Finance.Api.Data;
using Finance.Api.Helpers;

namespace Finance.Api.Controllers;

[ApiController]
[Route("api/expense")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly FinanceDbContext _context;
    public ExpensesController(FinanceDbContext context)
    {
        _context = context;
    }
//------------------------------------------------------------------------------------------------------------------------------------
//create expense endpoint
    [HttpPost("create")]
    public async Task<IActionResult> CreateExpense(CreateExpenseDto request){
        var userId = User.GetUserId();
        //verify category belongs to user
        var category  = await _context.Categories.FirstOrDefaultAsync(c=>c.Id== request.CategoryId && c.UserId==userId);
        if(category == null){
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid category" });
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
        return CreatedAtAction(nameof(CreateExpense), response);
    }
//------------------------------------------------------------------------------------------------------------------------------------
//get all expenses for the user

    [HttpGet("get")]
    public async Task<IActionResult> GetExpense(){
        var userId = User.GetUserId();
        var expenses = await _context.Expenses
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.UserId == userId)
            .ToListAsync();
        var expenseDtos = expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            Description = e.Notes,
            Amount = e.Amount,
            Date = e.ExpenseDate,
            CategoryId = e.CategoryId
        }).ToList();
        return Ok(expenseDtos);
    }
//------------------------------------------------------------------------------------------------------------------------------------
//update expense endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateExpense(int id, UpdateExpenseDto request){
        var userId = User.GetUserId();
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if(expense == null){
            return NotFound(new ApiResponse { Success = false, Message = "Expense not found" });
        }

        // Verify category belongs to user
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId);
        if(category == null){
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid category" });
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
        return Ok(response);
    }
//------------------------------------------------------------------------------------------------------------------------------------
//delete expense endpoint

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteExpense(int id){
        var userId = User.GetUserId();
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if(expense == null){
            return NotFound(new ApiResponse { Success = false, Message = "Expense not found" });
        }
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return Ok(new ApiResponse { Success = true, Message = "Expense deleted successfully" });
    }
//------------------------------------------------------------------------------------------------------------------------------------
//filter expense endpoint
    [HttpPost("filter")]
    public async Task<IActionResult> FilterExpense(FilterExpenseDto request){
        var userId = User.GetUserId();
        var query = _context.Expenses.AsNoTracking().Include(e => e.Category).Where(e => e.UserId == userId).AsQueryable();
        if(request.categoryId.HasValue){
            query = query.Where(e => e.CategoryId == request.categoryId.Value);
        }
        if(request.StartDate.HasValue){
            query = query.Where(e => e.ExpenseDate >= request.StartDate.Value);
        }
        if(request.EndDate.HasValue){
            query = query.Where(e => e.ExpenseDate <= request.EndDate.Value);
        }
        if(request.MinAmount.HasValue){
            query = query.Where(e => e.Amount >= request.MinAmount.Value);
        }
        if(request.MaxAmount.HasValue){
            query = query.Where(e => e.Amount <= request.MaxAmount.Value);
        }
        if(request.Month.HasValue){
            query = query.Where(e => e.ExpenseDate.Month == request.Month.Value);
        }
        if(request.Year.HasValue){
            query = query.Where(e => e.ExpenseDate.Year == request.Year.Value);
        }
        if(!string.IsNullOrEmpty(request.Notes)){
            query = query.Where(e => e.Notes.Contains(request.Notes));
        }

        var filteredExpenses = await query.ToListAsync();
        var expenseDtos = filteredExpenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            Description = e.Notes,
            Amount = e.Amount,
            Date = e.ExpenseDate,
            CategoryId = e.CategoryId
        }).ToList();
        return Ok(new ApiResponse<List<ExpenseDto>> { Success = true, Message = "Expense filtered successfully", Data = expenseDtos });
    }
}