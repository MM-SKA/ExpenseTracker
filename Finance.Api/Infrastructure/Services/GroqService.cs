using System.Net.Http.Headers;
using System.Text.Json;

using Finance.Api.Infrastructure.Options;

using Microsoft.Extensions.Options;

namespace Finance.Api.Infrastructure.Services;

internal sealed class GroqService(
    HttpClient httpClient,
    IOptions<GroqOptions> options)
    : IGroqService
{
    private readonly GroqOptions groqOptions =
        options.Value;

    public async Task<string> AskAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                groqOptions.ApiKey);

        var requestBody = new
        {
            model = groqOptions.Model,

            messages =
                new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
        };

        var response =
            await httpClient.PostAsJsonAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                requestBody,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        using var json =
            JsonDocument.Parse(
                await response.Content.ReadAsStringAsync(
                    cancellationToken));

        return json
            .RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
                ?? string.Empty;
    }
}
