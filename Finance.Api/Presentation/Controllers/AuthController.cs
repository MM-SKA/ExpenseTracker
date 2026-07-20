using Finance.Api.Application.Interfaces;
using Finance.Api.Application.DTOs.Auth;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Presentation.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
internal class AuthController(IAuthService authService) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequestDto request)
    {
        var response = await authService.RegisterAsync(request).ConfigureAwait(false);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(
            nameof(RegisterAsync),
            response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequestDto request)
    {
        var response = await authService.LoginAsync(request).ConfigureAwait(false);

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> MeAsync()
    {
        var userId = User.GetUserId();

        var response =
            await authService.GetCurrentUserAsync(userId).ConfigureAwait(false);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpPatch("update")]
    public async Task<IActionResult> UpdateUserAsync(UpdateUserDto request)
    {
        var userId = User.GetUserId();

        var response =
            await authService.UpdateUserAsync(userId, request).ConfigureAwait(false);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDto request)
    {
        var userId = User.GetUserId();

        var response =
            await authService.ChangePasswordAsync(userId, request).ConfigureAwait(false);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsersAsync()
    {
        var response = await authService.GetUsersAsync().ConfigureAwait(false);

        return Ok(response);
    }
}
