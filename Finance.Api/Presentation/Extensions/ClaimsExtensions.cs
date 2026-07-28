using System.Security.Claims;

namespace Finance.Api.Presentation.Extensions;

public static class ClaimsExtensions
{
    /// <summary>
    /// Gets the UserId from the current user's claims
    /// </summary>
    public static string GetUserId(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"JWT UserId = {userId}");
        if (!Guid.TryParse(userId, out _))
        {
            throw new InvalidOperationException("Invalid UserID.");
        }
        return userId;
    }
}
