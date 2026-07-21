using Asp.Versioning;

using Finance.Api.Application.DTOs.V2.Expenses;
using Finance.Api.Application.Interfaces.V2;
using Finance.Api.Presentation.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Presentation.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/expenses")]
[Route("api/expenses")]
[Authorize]
public class ExpensesV2Controller(IExpenseServiceV2<ExpenseDtoV2> expenseService) : ControllerBase
{
    [HttpGet("get")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetExpenseAsync()
    {
        var userId = User.GetUserId();
        return Ok(await expenseService.GetExpensesAsync(userId).ConfigureAwait(false));
    }
}
