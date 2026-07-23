using Asp.Versioning;

using Finance.Api.Application.DTOs.Common;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.V1.Expenses;
using Finance.Api.Application.Interfaces.V1;
using Finance.Api.Presentation.Extensions;
using Finance.Api.Application.Validation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Presentation.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses")]
[Route("api/expenses")]
[Authorize]
public class ExpensesController(IExpenseServiceV1<ExpenseDtoV1> expenseService, CreateExpenseValidator createExpenseValidator) : ControllerBase
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
        var validationResult = await createExpenseValidator.ValidateAsync(request).ConfigureAwait(false);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse { Success = false, Message = validationResult.Errors.First().ErrorMessage });
        }
        var userId = User.GetUserId();
        var result = await expenseService.CreateExpenseAsync(userId, request).ConfigureAwait(false);
        return CreatedAtAction(nameof(CreateExpenseAsync), result);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //get all expenses for the user

    [HttpGet("get")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetExpenseAsync()
    {
        var userId = User.GetUserId();
        return Ok(await expenseService.GetExpenseAsync(userId).ConfigureAwait(false));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //update expense endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateExpenseAsync(int id, UpdateExpenseDto request)
    {
        var userId = User.GetUserId();
        return Ok(await expenseService.UpdateExpenseAsync(userId, id, request).ConfigureAwait(false));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //delete expense endpoint

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteExpenseAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(await expenseService.DeleteExpenseAsync(userId, id).ConfigureAwait(false));
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
