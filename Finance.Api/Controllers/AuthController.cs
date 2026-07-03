using Finance.Api.Data;
using Finance.Api.Models;
using Finance.Api.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase{
    private readonly FinanceDbContext _context;

    public AuthController(FinanceDbContext context){
        _context=context;
    }

    //register endpoint
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request){
        // Implementation for user registration
        var user = new AppUser{
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User registered successfully");
    }

    //get all users endpoint
    [HttpGet("users")]
    public IActionResult GetUsers(){
        return Ok(_context.Users.ToList());
    }

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
        return Ok(new{
            Message = "Login Successful",
            fullName = user.FullName,
            email = user.Email
        });
    }
}