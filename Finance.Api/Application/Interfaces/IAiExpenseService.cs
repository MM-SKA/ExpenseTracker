using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.AI;

namespace Finance.Api.Application.Interfaces;

public interface IAiExpenseService
{
    Task<FilterExpenseDto> AiExtractFeaturesAsync(
        string userId,
        string question,
        CancellationToken cancellationToken);

    Task<AskExpenseAnswerDto> AiExecuteQuestionAsync(
        string userId,
        string question,
        CancellationToken cancellationToken);
}
