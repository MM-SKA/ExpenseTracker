using Finance.Api.Application.Interfaces;
using Finance.Api.Infrastructure.Data;
using Finance.Api.Application.DTOs.Auth;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Domain.Entities;
using Finance.Api.Application.Constants;
using FluentValidation;
using Finance.Api.Application.Logs.AppUser;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Finance.Api.Application.Services;

internal sealed class AuthService(IAuthRepository authRepository, IJWTService _jwtService, ILogger<AuthService> _logger, IValidator<RegisterRequestDto> validator, IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
    : IAuthService

{

    public async Task<ApiResponse<AuthDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var emailExists = await authRepository.EmailExistsAsync(request.Email.ToLowerInvariant(), cancellationToken).ConfigureAwait(false);

        if (emailExists)
        {
            AppUserServiceLogs.UserLogin(_logger, request.Email);

            return new ApiResponse<AuthDto>
            {
                Success = false,
                Message = "Email already exists",
                ErrorCode = ErrorCodes.DuplicateRequest
            };
        }

        var phoneExists = await authRepository.PhoneExistsAsync(request.PhoneNumber, cancellationToken).ConfigureAwait(false);

        if (phoneExists)
        {

            return new ApiResponse<AuthDto>
            {
                Success = false,
                Message = "Phone number already exists",
                ErrorCode = ErrorCodes.DuplicateRequest
            };
        }

        var StrongPassword = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        if (!StrongPassword.IsValid)
        {
            return new ApiResponse<AuthDto> { Success = false, Message = StrongPassword.Errors.First().ErrorMessage };
        }
        var user = new AppUser
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await authRepository.AddUserAsync(user, cancellationToken).ConfigureAwait(false);

        await authRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var authDto = new AuthDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        AppUserServiceLogs.UserRegisters(_logger, request.Email);

        return new ApiResponse<AuthDto>
        {
            Success = true,
            Message = "User registered successfully",
            Data = authDto
        };
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var user = await authRepository.GetUserByEmailAsync(request.Email, cancellationToken).ConfigureAwait(false);

        if (user == null)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Invalid email",
                ErrorCode = ErrorCodes.InvalidCredentials
            };
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            AppUserServiceLogs.InvalidAttempt(_logger, request.Email);
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Invalid password",
                ErrorCode = ErrorCodes.InvalidCredentials
            };
        }

        var accessToken =
            _jwtService.GenerateAccessToken(user);

        var refreshToken =
            _jwtService.GenerateRefreshToken();

        var refreshTokenHash =
            _jwtService.HashToken(refreshToken);

        var refreshTokenDays =
            configuration.GetValue<int>(
                "Jwt:RefreshTokenDays");

        var refreshTokenEntity =
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays)
            };

        await refreshTokenRepository
            .AddAsync(
                refreshTokenEntity,
                cancellationToken)
            .ConfigureAwait(false);

        await refreshTokenRepository
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        var authDto = new AuthDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        AppUserServiceLogs.UserLogin(_logger, request.Email);

        var loginResponse = new LoginResponseDto
        {
            User = authDto,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return new ApiResponse<LoginResponseDto>
        {
            Success = true,
            Message = "Login successful",
            Data = loginResponse
        };
    }

    public async Task<ApiResponse<List<AuthDto>>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await authRepository.GetUsersAsync(cancellationToken).ConfigureAwait(false);

        var authDtos = users.Select(u => new AuthDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber
        })
            .ToList();
        return new ApiResponse<List<AuthDto>>
        {
            Success = true,
            Message = "Users fetched successfully",
            Data = authDtos
        };
    }

    public async Task<ApiResponse<AuthDto>> GetCurrentUserAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await authRepository.GetUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);

        if (user == null)
        {

            return new ApiResponse<AuthDto>
            {
                Success = false,
                Message = "User not found",
                ErrorCode = ErrorCodes.UserNotFound
            };
        }

        var authDto = new AuthDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return new ApiResponse<AuthDto>
        {
            Success = true,
            Message = "User fetched successfully",
            Data = authDto
        };
    }

    public async Task<ApiResponse<AuthDto>> UpdateUserAsync(string userId, UpdateUserDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var user = await authRepository.GetUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);

        if (user == null)
        {

            return new ApiResponse<AuthDto>
            {
                Success = false,
                Message = "User not found",
                ErrorCode = ErrorCodes.UserNotFound
            };
        }

        if (!string.IsNullOrWhiteSpace(request.Email) &&
            request.Email != user.Email)
        {
            var emailExists = await authRepository.EmailExistsAsync(request.Email, cancellationToken).ConfigureAwait(false);

            if (emailExists)
            {
                return new ApiResponse<AuthDto>
                {
                    Success = false,
                    Message = "Email already exists",
                    ErrorCode = ErrorCodes.DuplicateRequest
                };
            }

            user.Email = request.Email.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) &&
            request.PhoneNumber != user.PhoneNumber)
        {
            var phoneExists = await authRepository.PhoneExistsAsync(request.PhoneNumber, cancellationToken).ConfigureAwait(false);

            if (phoneExists)
            {
                return new ApiResponse<AuthDto>
                {
                    Success = false,
                    Message = "Phone number already exists",
                    ErrorCode = ErrorCodes.DuplicateRequest
                };
            }

            user.PhoneNumber = request.PhoneNumber.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            user.FullName = request.FullName.Trim();
        }

        await authRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var authDto = new AuthDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return new ApiResponse<AuthDto>
        {
            Success = true,
            Message = "User updated successfully",
            Data = authDto
        };
    }

    public async Task<ApiResponse> ChangePasswordAsync(
        string userId,
        ChangePasswordDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var user = await authRepository.GetUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);

        if (user == null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = "User not found",
                ErrorCode = ErrorCodes.UserNotFound
            };
        }

        var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.CurrentPassword,
            user.PasswordHash);

        if (!isCurrentPasswordValid)
        {

            return new ApiResponse
            {
                Success = false,
                Message = "Enter the correct password",
                ErrorCode = ErrorCodes.InvalidPasswordWhileChangingPassword
            };
        }

        if (request.CurrentPassword == request.NewPassword)
        {
            return new ApiResponse
            {
                Success = false,
                Message = "New password must be different from current password",
                ErrorCode = ErrorCodes.NewPasswordMatchesOldPassword
            };
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
            request.NewPassword);

        await authRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        AppUserServiceLogs.PasswordUpdateSuccess(_logger, user.Email);

        return new ApiResponse
        {
            Success = true,
            Message = "Password updated successfully"
        };
    }

    public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Refresh token is required",
                ErrorCode = ErrorCodes.InvalidCredentials
            };
        }

        var tokenHash = _jwtService.HashToken(refreshToken);

        var existingToken = await refreshTokenRepository
            .GetByTokenHashAsync(tokenHash, cancellationToken)
            .ConfigureAwait(false);

        if (existingToken == null)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Invalid refresh token",
                ErrorCode = ErrorCodes.InvalidCredentials
            };
        }

        // Reuse detection: if the token is already revoked, revoke the entire family
        if (existingToken.RevokedAt != null)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Refresh token has been revoked. Please log in again.",
                ErrorCode = ErrorCodes.InvalidCredentials
            };
        }

        // Check expiry
        if (existingToken.ExpiresAt <= DateTime.UtcNow)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Refresh token has expired. Please log in again.",
                ErrorCode = ErrorCodes.InvalidCredentials
            };
        }

        // Fetch the user
        var user = await authRepository
            .GetUserByIdAsync(existingToken.UserId, cancellationToken)
            .ConfigureAwait(false);

        if (user == null)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "User not found",
                ErrorCode = ErrorCodes.UserNotFound
            };
        }

        var newAccessToken = _jwtService.GenerateAccessToken(user);

        var authDto = new AuthDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        // AppUserServiceLogs.AccessTokenRefreshed(_logger, user.Id);

        return new ApiResponse<LoginResponseDto>
        {
            Success = true,
            Message = "Token refreshed successfully",
            Data = new LoginResponseDto
            {
                User = authDto,
                AccessToken = newAccessToken,
                RefreshToken = refreshToken
            }
        };
    }

    public async Task<ApiResponse> LogoutAsync(string? refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new ApiResponse
            {
                Success = true,
                Message = "Logged out"
            };
        }

        var tokenHash = _jwtService.HashToken(refreshToken);

        var existingToken = await refreshTokenRepository
            .GetByTokenHashAsync(tokenHash, cancellationToken)
            .ConfigureAwait(false);

        if (existingToken != null && existingToken.RevokedAt == null)
        {
            existingToken.RevokedAt = DateTime.UtcNow;

            await refreshTokenRepository
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        AppUserServiceLogs.UserLoggedOut(_logger);

        return new ApiResponse
        {
            Success = true,
            Message = "Logged out successfully"
        };
    }
}
