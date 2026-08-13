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
    public async Task<IActionResult> CreateExpenseAsync(CreateExpenseDto request, CancellationToken cancellationToken)
    {
        var validationResult = await createExpenseValidator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse { Success = false, Message = validationResult.Errors.First().ErrorMessage });
        }
        var userId = User.GetUserId();
        var result = await expenseService.CreateExpenseAsync(userId, request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //get all expenses for the user

    [HttpGet("get")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetExpenseAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        return Ok(await expenseService.GetExpenseAsync(userId, cancellationToken).ConfigureAwait(false));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //get expense by id endpoint

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetExpenseByIdAsync(
    string id,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await expenseService
            .GetExpenseByIdAsync(
                userId,
                id,
                cancellationToken).ConfigureAwait(false);

        return Ok(result);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //update expense endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateExpenseAsync(string id, UpdateExpenseDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        return Ok(await expenseService.UpdateExpenseAsync(userId, id, request, cancellationToken).ConfigureAwait(false));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //delete expense endpoint

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteExpenseAsync(string id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        return Ok(await expenseService.DeleteExpenseAsync(userId, id, cancellationToken).ConfigureAwait(false));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //filter expense endpoint

    // [HttpPost("filter")]
    // public async Task<IActionResult> FilterExpenseAsync(FilterExpenseDto request, CancellationToken cancellationToken)
    // {
    //     var userId = User.GetUserId();
    //     var response = await expenseService.FilterExpensesWithAnalyticsAsync(userId, request, cancellationToken).ConfigureAwait(false);
    //     return Ok(new ApiResponse<FilterResponseDto> { Success = true, Message = "Expense filtered successfully", Data = response });
    // }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedExpensesAsync([FromQuery] PaginationRequestDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var userId = User.GetUserId();

        var response =
            await expenseService.GetPaginatedExpensesAsync(userId, request.PageNumber, request.PageSize, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetSearchedExpenseAsync([FromQuery] SearchRequestDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await expenseService.GlobalSearchAsync(userId, request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPost("filter-paged")]
    public async Task<IActionResult> FilterPagedExpensesAsync(
    FilterExpenseDto request,
    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId =
            User.GetUserId();

        var response =
            await expenseService
                .FilterPagedExpensesAsync(
                    userId,
                    request,
                    cancellationToken)
                .ConfigureAwait(false);

        return Ok(new ApiResponse<FilteredPagedExpenseResponseDto>
        {
            Success = true,
            Message = "Expenses filtered successfully",
            Data = response
        });
    }

    [HttpPost("export")]
    public async Task<IActionResult> ExportExpensesAsync(
    FilterExpenseDto request,
    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId =
            User.GetUserId();

        var fileBytes =
            await expenseService
                .ExportExpensesToExcelAsync(
                    userId,
                    request,
                    cancellationToken)
                .ConfigureAwait(false);

        var fileName =
            $"expenses-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
