using System.Text.Json;

using Finance.Api.Application.DTOs.AI;
using Finance.Api.Application.DTOs.Expenses;
using Finance.Api.Application.DTOs.V1.Expenses;
using Finance.Api.Application.Interfaces;
using Finance.Api.Application.Interfaces.V1;

namespace Finance.Api.Application.Services;

internal sealed class AiExpenseService(
    IExpenseServiceV1<ExpenseDtoV1> expenseService,
    ICategoryService categoryService,
    IGroqService groqAiService)
    : IAiExpenseService
{
    private static readonly JsonSerializerOptions DeserializeOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

    private static readonly JsonSerializerOptions SerializeOptions =
        new()
        {
            WriteIndented = true
        };

    public async Task<FilterExpenseDto> AiExtractFeaturesAsync(
        string userId,
        string question,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(question);

        var categories =
            await categoryService
                .GetCategoriesAsync(
                    userId,
                    cancellationToken)
                .ConfigureAwait(false);

        var categoryPrompt =
            string.Join(
                Environment.NewLine,
                categories.Select(c => $"- {c.Name}"));

        var prompt =
            $$"""
            You are converting a user's expense question into JSON filters.

            Available categories:
            {{categoryPrompt}}

            User question:
            {{question}}

            Return ONLY raw JSON.
            Do NOT wrap the response in markdown.
            Do NOT use ```json.
            Do NOT add explanation.

            Return JSON with exactly these properties:

            {
              "categoryId": null,
              "startDate": null,
              "endDate": null,
              "minAmount": null,
              "maxAmount": null,
              "month": null,
              "year": null,
              "notes": null,
              "location": null,
              "includeAnalytics": true
            }

            Rules:
            - categoryId must contain the category NAME, not the category GUID.
            - If user says Food, categoryId should be "Food".
            - If user says Travel, categoryId should be "Travel".
            - month must be a number from 1 to 12.
            - Do not return month names like "July".
            - for end date : include that day till 11:59:59 pm
            - year must be a number like 2026.
            - startDate and endDate must be ISO dates or null.
            - minAmount and maxAmount must be numbers or null.
            - includeAnalytics should be true.
            """;

        var llmResponse =
            await groqAiService
                .AskAsync(
                    prompt,
                    cancellationToken)
                .ConfigureAwait(false);

        var cleanJson =
            ExtractJson(llmResponse);

        var filters =
            JsonSerializer.Deserialize<FilterExpenseDto>(
                cleanJson,
                DeserializeOptions)
            ?? new FilterExpenseDto();

        if (!string.IsNullOrWhiteSpace(filters.CategoryId))
        {
            var category =
                categories.FirstOrDefault(c =>
                    string.Equals(
                        c.Name,
                        filters.CategoryId,
                        StringComparison.OrdinalIgnoreCase));

            filters.CategoryId = category?.Id;
        }

        filters.IncludeAnalytics = true;

        return filters;
    }

    public async Task<AskExpenseAnswerDto> AiExecuteQuestionAsync(
        string userId,
        string question,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(question);

        var filters =
            await AiExtractFeaturesAsync(
                userId,
                question,
                cancellationToken)
            .ConfigureAwait(false);

        filters.PageNumber = 1;
        filters.PageSize = 100;
        filters.IncludeAnalytics = true;

        var result =
            await expenseService
                .FilterPagedExpensesAsync(
                    userId,
                    filters,
                    cancellationToken)
                .ConfigureAwait(false);

        var resultJson =
            JsonSerializer.Serialize(
                result,
                SerializeOptions);

        var answerPrompt =
            $$"""
            You are a personal finance assistant.

            User question:
            {{question}}

            Database result:
            {{resultJson}}

            Rules:
            - Answer only using the supplied database result.
            - Do not invent numbers.
            - If no expenses are found, clearly say no matching expenses were found.
            - Use analytics when available.
            - Keep the answer concise and useful.
            - Mention total amount, count, and average if available.
            """;

        var answer =
            await groqAiService
                .AskAsync(
                    answerPrompt,
                    cancellationToken)
                .ConfigureAwait(false);

        return new AskExpenseAnswerDto
        {
            Answer = answer.Trim(),
            FiltersUsed = filters,
            Analytics = result.Analytics
        };
    }

    private static string ExtractJson(
        string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return "{}";
        }

        var cleaned =
            response
                .Replace(
                    "```json",
                    string.Empty,
                    StringComparison.OrdinalIgnoreCase)
                .Replace(
                    "```",
                    string.Empty,
                    StringComparison.Ordinal)
                .Trim();

        var firstBrace =
            cleaned.IndexOf('{');

        var lastBrace =
            cleaned.LastIndexOf('}');

        if (firstBrace >= 0 &&
            lastBrace >= firstBrace)
        {
            return cleaned[
                firstBrace..(lastBrace + 1)];
        }

        return cleaned;
    }
}
