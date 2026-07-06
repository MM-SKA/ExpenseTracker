using System.Security.Claims;

namespace Finance.Api.Helpers;

public static class ClaimsExtensions
{
    /// <summary>
    /// Gets the UserId from the current user's claims
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException("User ID not found in claims or invalid format");
        }
        
        return userId;
    }
}
