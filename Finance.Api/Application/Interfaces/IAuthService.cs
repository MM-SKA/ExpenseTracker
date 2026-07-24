using Finance.Api.Application.DTOs.Auth;
using Finance.Api.Application.DTOs.Common;

namespace Finance.Api.Application.Interfaces;

public interface IAuthService
{

    Task<ApiResponse<AuthDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);

    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);

    Task<ApiResponse<List<AuthDto>>> GetUsersAsync(CancellationToken cancellationToken);

    Task<ApiResponse<AuthDto>> GetCurrentUserAsync(int userId, CancellationToken cancellationToken);

    Task<ApiResponse<AuthDto>> UpdateUserAsync(int userId, UpdateUserDto request, CancellationToken cancellationToken);

    Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto request, CancellationToken cancellationToken);

}
