using Finance.Api.Application.DTOs.AI;
using Finance.Api.Application.Interfaces;
using Finance.Api.Presentation.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
        var userId = User.GetUserId();

        //api to send question to llm and receive filters
        var filters = await aiExpenseService.ExtractFiltersAsync(userId, request.Question, cancellationToken).ConfigureAwait(false);
        return Ok(filters);

    }
}
