public interface IGroqService
{
    Task<string> AskAsync(
        string prompt,
        CancellationToken cancellationToken);
}
