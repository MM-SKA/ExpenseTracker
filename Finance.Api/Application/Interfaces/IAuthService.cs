using Finance.Api.Application.DTOs.Auth;
using Finance.Api.Application.DTOs.Common;

namespace Finance.Api.Application.Interfaces;

public interface IAuthService
{

    Task<ApiResponse<AuthDto>> RegisterAsync(RegisterRequestDto request);

    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);

    Task<ApiResponse<List<AuthDto>>> GetUsersAsync();

    Task<ApiResponse<AuthDto>> GetCurrentUserAsync(int userId);

    Task<ApiResponse<AuthDto>> UpdateUserAsync(int userId, UpdateUserDto request);

    Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto request);

}
