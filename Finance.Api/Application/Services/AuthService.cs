using Finance.Api.Application.Interfaces;
using Finance.Api.Data;
using Finance.Api.Application.DTOs.Auth;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Application.Services;

public class AuthService : IAuthService
{
    private readonly FinanceDbContext _context;
    private readonly IJWTService _jwtService;

    public AuthService(
        FinanceDbContext context,
        IJWTService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<AuthDto>> RegisterAsync(RegisterRequestDto request)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
        {
            return new ApiResponse<AuthDto>
            {
                Success = false,
                Message = "Email already exists"
            };
        }

        var phoneExists = await _context.Users
            .AnyAsync(u => u.PhoneNumber == request.PhoneNumber);

        if (phoneExists)
        {
            return new ApiResponse<AuthDto>
            {
                Success = false,
                Message = "Phone number already exists"
            };
        }

        var user = new AppUser
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

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
            Message = "User registered successfully",
            Data = authDto
        };
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Invalid email"
            };
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Invalid password"
            };
        }

        var token = _jwtService.GenerateToken(user);

        var authDto = new AuthDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        var loginResponse = new LoginResponseDto
        {
            User = authDto,
            Token = token
        };

        return new ApiResponse<LoginResponseDto>
        {
            Success = true,
            Message = "Login successful",
            Data = loginResponse
        };
    }

    public async Task<ApiResponse<List<AuthDto>>> GetUsersAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new AuthDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            })
            .ToListAsync();

        return new ApiResponse<List<AuthDto>>
        {
            Success = true,
            Message = "Users fetched successfully",
            Data = users
        };
    }

    public async Task<ApiResponse<AuthDto>> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new ApiResponse<AuthDto>
            {
                Success = false,
                Message = "User not found"
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

    public async Task<ApiResponse<AuthDto>> UpdateUserAsync(
        int userId,
        UpdateUserDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new ApiResponse<AuthDto>
            {
                Success = false,
                Message = "User not found"
            };
        }

        if (!string.IsNullOrWhiteSpace(request.Email) &&
            request.Email != user.Email)
        {
            var emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Email == request.Email &&
                    u.Id != userId);

            if (emailExists)
            {
                return new ApiResponse<AuthDto>
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            user.Email = request.Email.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) &&
            request.PhoneNumber != user.PhoneNumber)
        {
            var phoneExists = await _context.Users
                .AnyAsync(u =>
                    u.PhoneNumber == request.PhoneNumber &&
                    u.Id != userId);

            if (phoneExists)
            {
                return new ApiResponse<AuthDto>
                {
                    Success = false,
                    Message = "Phone number already exists"
                };
            }

            user.PhoneNumber = request.PhoneNumber.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            user.FullName = request.FullName.Trim();
        }

        await _context.SaveChangesAsync();

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
        int userId,
        ChangePasswordDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = "User not found"
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
                Message = "Enter the correct password"
            };
        }

        if (request.CurrentPassword == request.NewPassword)
        {
            return new ApiResponse
            {
                Success = false,
                Message = "New password must be different from current password"
            };
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
            request.NewPassword);

        await _context.SaveChangesAsync();

        return new ApiResponse
        {
            Success = true,
            Message = "Password updated successfully"
        };
    }
}