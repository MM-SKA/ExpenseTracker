using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Domain.Entities;
using Finance.Api.Infrastructure.Data;
using Finance.Api.Presentation.Extensions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Presentation;
using Finance.Api.Application.Interfaces;

namespace Finance.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController(IExpenseService<ExpenseDto> expenseService) : ControllerBase
{
    //------------------------------------------------------------------------------------------------------------------------------------
    //create expense endpoint
    [HttpPost("test")]
    public IActionResult TestRequest(CreateExpenseUsingHeaderDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Ok(new
        {
            ClientId = Request.Headers["X-Client-Id"],
            ApiVersion = Request.Headers["X-Api-Version"],
            Description = request.Desc,
            Amount = request.Amount
        });
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //create expense endpoint
    [HttpPost("create")]
    public async Task<IActionResult> CreateExpenseAsync(CreateExpenseDto request)
    {
        var userId = User.GetUserId();
        var result = expenseService.CreateExpense(userId, request);
        return CreatedAtAction(nameof(CreateExpenseAsync), result);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //get all expenses for the user

    [HttpGet("get")]
    public async Task<IActionResult> GetExpenseAsync()
    {
        var userId = User.GetUserId();
        return Ok(expenseService.GetExpense(userId));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //update expense endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateExpenseAsync(int id, UpdateExpenseDto request)
    {
        var userId = User.GetUserId();
        return Ok(expenseService.UpdateExpense(userId, id, request));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //delete expense endpoint

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteExpenseAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(expenseService.DeleteExpense(userId, id));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //filter expense endpoint

    [HttpPost("filter")]
    public async Task<IActionResult> FilterExpenseAsync(FilterExpenseDto request)
    {
        var userId = User.GetUserId();
        var response = await expenseService.FilterExpensesWithAnalyticsAsync(userId, request).ConfigureAwait(false);
        return Ok(new ApiResponse<FilterResponseDto> { Success = true, Message = "Expense filtered successfully", Data = response });
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedExpensesAsync([FromQuery] PaginationRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var userId = User.GetUserId();

        var response =
            await expenseService.GetPaginatedExpensesAsync(userId, request.PageNumber, request.PageSize).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetSearchedExpenseAsync([FromQuery] SearchRequestDto request)
    {
        var userId = User.GetUserId();
        var result = await expenseService.GlobalSearchAsync(userId, request).ConfigureAwait(false);
        return Ok(result);
    }
}
