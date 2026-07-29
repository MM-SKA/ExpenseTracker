using Finance.Api.Application.DTOs.AI;
using Finance.Api.Application.Interfaces;
using Finance.Api.Presentation.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Presentation.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public sealed class AiController(
    IAiExpenseService aiExpenseService)
    : ControllerBase
{
    [HttpPost("ask-expenses")]
    public async Task<IActionResult> AskExpensesAsync(
        AskExpenseQuestionDto request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = User.GetUserId();

        var answer =
            await aiExpenseService
                .AiExecuteQuestionAsync(
                    userId,
                    request.Question,
                    cancellationToken)
                .ConfigureAwait(false);

        return Ok(answer);
    }
}
