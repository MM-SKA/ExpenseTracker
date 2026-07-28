using System.Text.Json;

using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.Interfaces;

namespace Finance.Api.Application.Services;

internal sealed class AiExpenseService(
    ICategoryService categoryService,
    IGroqService groqAiService)
    : IAiExpenseService
{
    public async Task<FilterExpenseDto> ExtractFiltersAsync(
        string userId,
        string question,
        CancellationToken cancellationToken)
    {
        var categories =
            await categoryService
                .GetCategoriesAsync(
                    userId,
                    cancellationToken)
                .ConfigureAwait(false);

        var categoryPrompt =
            string.Join(
                Environment.NewLine,
                categories.Select(c => c.Name));

        var prompt =
            $$"""
            You are converting a user expense query into JSON.

            Available categories:

            {{categoryPrompt}}

            Question:
            {{question}}

            Return ONLY valid JSON matching:

            { "categoryId": "string or null", "startDate": "ISO date or null", "endDate": "ISO date or null", "minAmount": "number or null", "maxAmount": "number or null", "month": "integer or null", "year": "integer or null", "notes": "string or null", "location": "string or null", "includeAnalytics": true }

            IMPORTANT:

            - Return category NAME inside categoryId temporarily.
            - Do not add explanations.
            - Return JSON only.
            """;

        var llmResponse =
            await groqAiService
                .AskAsync(
                    prompt,
                    cancellationToken)
                .ConfigureAwait(false);
        llmResponse = llmResponse.Trim();

        if (llmResponse.StartsWith("```"))
        {
            llmResponse = llmResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();
        }

        Console.WriteLine(llmResponse);
        var filters =
            JsonSerializer.Deserialize<FilterExpenseDto>(
                llmResponse,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return filters ?? new FilterExpenseDto();
    }
}
