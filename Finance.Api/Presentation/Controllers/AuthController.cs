using Finance.Api.Application.Interfaces;
using Finance.Api.Application.DTOs.Auth;
using Finance.Api.Presentation.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Finance.Api.Application.DTOs.Common;

namespace Finance.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        if (response.Success && response.Data is not null)
        {
            SetAuthCookies(response.Data.AccessToken, response.Data.RefreshToken);
            response.Data.AccessToken = string.Empty;
            response.Data.RefreshToken = string.Empty;
        }

        return Ok(new ApiResponse<AuthDto>
        {
            Success = true,
            Message = "Login successful",
            Data = response.Data.User
        });
    }

    [Authorize]
    [HttpPatch("update")]
    public async Task<IActionResult> UpdateUserAsync(UpdateUserDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var response =
            await authService.UpdateUserAsync(userId, request, cancellationToken).ConfigureAwait(false);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var response =
            await authService.ChangePasswordAsync(userId, request, cancellationToken).ConfigureAwait(false);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsersAsync(CancellationToken cancellationToken)
    {
        var response = await authService.GetUsersAsync(cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    private void SetAuthCookies(
    string accessToken,
    string refreshToken)
    {
        Response.Cookies.Append(
            "accessToken",
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddSeconds(10)
            });

        Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(14)
            });
    }

    private void ClearAuthCookies()
    {
        // Response.Cookies.Append(
        //     "accessToken",
        //     string.Empty,
        //     new CookieOptions
        //     {
        //         HttpOnly = true,
        //         Secure = true,
        //         SameSite = SameSiteMode.None,
        //         Expires = DateTimeOffset.UtcNow.AddDays(-1)
        //     });

        Response.Cookies.Delete(
        "accessToken",
        new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });

        Response.Cookies.Delete(
        "refreshToken",
        new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var response = await authService.RefreshTokenAsync(refreshToken, cancellationToken).ConfigureAwait(false);

        if (!response.Success)
        {
            ClearAuthCookies();
            return Unauthorized(response);
        }

        if (response.Data is not null)
        {
            SetAccessTokenCookie(response.Data.AccessToken);
            response.Data.AccessToken = string.Empty;
            response.Data.RefreshToken = string.Empty;
        }

        return Ok(new ApiResponse<AuthDto>
        {
            Success = true,
            Message = "Token refreshed successfully",
            Data = response.Data?.User
        });
    }

    private void SetAccessTokenCookie(string accessToken)
    {
        Response.Cookies.Append(
            "accessToken",
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddSeconds(10),
                Path = "/"
            });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        _ = await authService.LogoutAsync(refreshToken, cancellationToken).ConfigureAwait(false);

        ClearAuthCookies();

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Logged out successfully"
        });
    }
}
