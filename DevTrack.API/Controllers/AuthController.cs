using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;
using Microsoft.Extensions.Logging;

namespace DevTrack.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly string _jwtKey;
    private readonly int _jwtExpirationHours;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _context = context;
        _jwtKey = configuration["Jwt:Key"]!;
        _jwtExpirationHours = int.Parse(configuration["Jwt:ExpirationHours"]!);
        _logger = logger;
    }

    [HttpPost("register")]
public IActionResult Register(RegisterDto dto)
{
    _logger.LogInformation($"Register request received for {dto.Email}");

    if (_context.Users.Any(u => u.Email == dto.Email))
    {
        _logger.LogWarning($"User {dto.Email} already exists.");
        return BadRequest("User already exists");
    }

    try
    {
        _logger.LogInformation($"Creating user: {dto.Email}");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Name = dto.Name,  // Adicionei Name aqui
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        _logger.LogInformation($"User {dto.Email} registered successfully.");
        return Ok("User registered");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error registering user: {dto.Email}");
        return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
    }
}


    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        _logger.LogInformation($"Login request received for {dto.Email}");

        var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
        if (user == null)
        {
            _logger.LogWarning($"Login failed: user with email {dto.Email} not found.");
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!valid)
        {
            _logger.LogWarning($"Login failed: invalid password for user {dto.Email}.");
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtExpirationHours),
            signingCredentials: creds
        );

        _logger.LogInformation($"Login successful: token generated for user {dto.Email}");

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}
