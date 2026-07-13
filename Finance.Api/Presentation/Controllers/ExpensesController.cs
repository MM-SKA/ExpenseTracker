using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Finance.Api.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Models;
using Finance.Api.Data;
using Finance.Api.Helpers;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Presentation;

namespace Finance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly FinanceDbContext _context;
    private readonly IExpenseAnalyticsService _analyticsService;

    public ExpensesController(FinanceDbContext context, IExpenseAnalyticsService analyticsService)
    {
        _context = context;
        _analyticsService = analyticsService;
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //create expense endpoint
    [HttpPost("create")]
    public async Task<IActionResult> CreateExpense(CreateExpenseDto request)
    {
        var userId = User.GetUserId();
        //verify category belongs to user
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && (c.IsSystemCategory || c.UserId == userId));
        if (category == null)
        {
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
    public async Task<IActionResult> GetExpense()
    {
        var userId = User.GetUserId();
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
        return Ok(expenseDtos);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //update expense endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateExpense(int id, UpdateExpenseDto request)
    {
        var userId = User.GetUserId();
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense == null)
        {
            return NotFound(new ApiResponse { Success = false, Message = "Expense not found" });
        }

        // Verify category belongs to user
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId);
        if (category == null)
        {
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
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var userId = User.GetUserId();
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense == null)
        {
            return NotFound(new ApiResponse { Success = false, Message = "Expense not found" });
        }
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return Ok(new ApiResponse { Success = true, Message = "Expense deleted successfully" });
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //filter expense endpoint

    [HttpPost("filter")]
    public async Task<IActionResult> FilterExpense(FilterExpenseDto request)
    {
        var userId = User.GetUserId();
        var response = await _analyticsService.FilterExpensesWithAnalyticsAsync(userId, request);
        return Ok(new ApiResponse<FilterResponseDto> { Success = true, Message = "Expense filtered successfully", Data = response });
    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //export expense endpoint
    [HttpGet("Export")]
    public async Task<IActionResult> ExportExpense()
    {
        //get the user id
        var userId = User.GetUserId();
        //get the user's expenses
        var expenses = await _context.Expenses
                                    .AsNoTracking()
                                    .Include(e => e.Category)
                                    .Where(e => e.UserId == userId)
                                    .OrderByDescending(e => e.ExpenseDate)
                                    .ToListAsync();
        //create a workbook
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Expenses");
        //add columns
        worksheet.Cell(1, 1).Value = "Category";
        worksheet.Cell(1, 2).Value = "Amount";
        worksheet.Cell(1, 3).Value = "Expense Date";
        worksheet.Cell(1, 4).Value = "Notes";
        worksheet.Cell(1, 5).Value = "Created At";
        //insert data
        int row = 2;
        foreach (var expense in expenses)
        {
            worksheet.Cell(row, 1).Value = expense.Category.Name;
            worksheet.Cell(row, 2).Value = expense.Amount;
            worksheet.Cell(row, 3).Value = expense.ExpenseDate;
            worksheet.Cell(row, 4).Value = expense.Notes ?? "";
            worksheet.Cell(row, 5).Value = expense.CreatedAt;
            row++;
        }
        //header styling
        var headerRange = worksheet.Range("A1:E1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        worksheet.Columns().AdjustToContents();
        worksheet.Cell(row + 1, 1).Value = "Total";
        //total
        worksheet.Cell(row + 1, 2).Value =
        expenses.Sum(e => e.Amount);

        worksheet.Cell(row + 1, 1).Style.Font.Bold = true;
        worksheet.Cell(row + 1, 2).Style.Font.Bold = true;

        //save in the memory stream
        using var stream = new MemoryStream();
        workbook.SaveAs("Test.xlsx");
        stream.Position=0;

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Expenses_{DateTime.UtcNow:yyyyMMdd}.xlsx"
        );
    }
}