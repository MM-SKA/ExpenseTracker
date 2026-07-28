namespace Finance.Api.Infrastructure.Options;

public sealed class GroqOptions
{
    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;
}
