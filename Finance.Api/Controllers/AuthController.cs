using Finance.Api.Data;
using Finance.Api.Models;
using Finance.Api.DTOs.Auth;
using Finance.Api.DTOs.Common;
using Finance.Api.Services;
using Finance.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Finance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase{
    private readonly FinanceDbContext _context;
    private readonly IJWTService _jwtService;

    public AuthController(FinanceDbContext context, IJWTService jwtService){
        _context=context;
        _jwtService=jwtService;
    }
    
//----------------------------------------------------------------------------------------
    //register endpoint
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request){
        var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);

        if(exists){
            return BadRequest(new ApiResponse { Success = false, Message = "Email already exists" });
        }

        var user = new AppUser{
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var authDto = new AuthDto { Id = user.Id, FullName = user.FullName, Email = user.Email };
        var response = new ApiResponse<AuthDto> { Success = true, Message = "User registered successfully", Data = authDto };
        return CreatedAtAction(nameof(Register), response);
    }
//----------------------------------------------------------------------------------------
    //get all users endpoint
    [HttpGet("users")]
    public IActionResult GetUsers(){
        var userDtos = _context.Users.AsNoTracking().Select(u => new AuthDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email
        }).ToList();
        return Ok(userDtos);
    }
//----------------------------------------------------------------------------------------
    //login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request){
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.Email== request.Email);
        if(user==null){
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid email" });
        }
        if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)){
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid password" });
        }
        
        var token =_jwtService.GenerateToken(user);
        var authDto = new AuthDto { Id = user.Id, FullName = user.FullName, Email = user.Email };
        var response = new ApiResponse<object> { Success = true, Message = "Login successful", Data = new { user = authDto, token } };

        return Ok(response);
    }
//----------------------------------------------------------------------------------------    
    //checking authorized endpoint
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.GetUserId();
        var name = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        
        var authDto = new AuthDto { Id = userId, FullName = name, Email = email };
        return Ok(authDto);
    }

}