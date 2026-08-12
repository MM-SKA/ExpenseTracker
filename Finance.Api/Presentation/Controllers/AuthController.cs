using Finance.Api.Application.Interfaces;
using Finance.Api.Application.DTOs.Auth;
using Finance.Api.Presentation.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Infrastructure.Services;

namespace Finance.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IJWTService jWtService, IRefreshTokenRepository refreshTokenRepository) : ControllerBase
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
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
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
}
