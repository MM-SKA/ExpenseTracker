using Finance.Api.Data;
using Finance.Api.Models;
using Finance.Api.DTOs.Auth;
using Finance.Api.Services;
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
        // Implementation for user registration
        var user = new AppUser{
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);

        if(exists){
            return BadRequest("Email already exists");
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User registered successfully");
    }
//----------------------------------------------------------------------------------------
    //get all users endpoint
    [HttpGet("users")]
    public IActionResult GetUsers(){
        return Ok( 
            _context.Users.Select(u => new{
                u.Id,
                u.FullName,
                u.Email
            })
        );
    }
//----------------------------------------------------------------------------------------
    //login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request){
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.Email== request.Email);
        //user not found
        if(user==null){
            return Unauthorized("Invalid email");
        }
        //check password
        if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)){
            return Unauthorized("Invalid password");
        }
        
        var token =_jwtService.GenerateToken(user);

        return Ok(new{
            Message = "Login Successful",
            fullName = user.FullName,
            email = user.Email,
            token = token
        });
    }
//----------------------------------------------------------------------------------------    
    //checking authorized endpoint
    [Authorize]
    [HttpGet("me")]

    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value,

            Name = User.FindFirst(
                ClaimTypes.Name
            )?.Value,

            Email = User.FindFirst(
                ClaimTypes.Email
            )?.Value
        });
    }

}