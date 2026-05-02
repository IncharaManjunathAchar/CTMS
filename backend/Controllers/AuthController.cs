using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _db.Passengers.AnyAsync(p => p.Email == dto.Email))
            return BadRequest("Email already registered.");

        if (await _db.Passengers.AnyAsync(p => p.MobileNumber == dto.MobileNumber))
            return BadRequest("Mobile number already registered.");

        var passenger = new Passenger
        {
            Name = dto.Name,
            Email = dto.Email,
            MobileNumber = dto.MobileNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PassengerType = dto.PassengerType
        };

        _db.Passengers.Add(passenger);
        await _db.SaveChangesAsync();
        return Ok("Registration successful.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var passenger = await _db.Passengers.FirstOrDefaultAsync(p => p.Email == dto.Email);
        if (passenger == null || !BCrypt.Net.BCrypt.Verify(dto.Password, passenger.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var token = GenerateToken(passenger);
        return Ok(new AuthResponseDto(token, passenger.Name, passenger.Email, passenger.PassengerType));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var passenger = await _db.Passengers.FindAsync(id);
        if (passenger == null) return NotFound();

        return Ok(new { passenger.Id, passenger.Name, passenger.Email, passenger.MobileNumber, passenger.PassengerType, passenger.RegisteredAt });
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var passenger = await _db.Passengers.FindAsync(id);
        if (passenger == null) return NotFound();

        passenger.Name = dto.Name;
        passenger.MobileNumber = dto.MobileNumber;
        passenger.PassengerType = dto.PassengerType;
        await _db.SaveChangesAsync();
        return Ok("Profile updated.");
    }

    private string GenerateToken(Passenger passenger)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, passenger.Id.ToString()),
            new Claim(ClaimTypes.Email, passenger.Email),
            new Claim(ClaimTypes.Name, passenger.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
