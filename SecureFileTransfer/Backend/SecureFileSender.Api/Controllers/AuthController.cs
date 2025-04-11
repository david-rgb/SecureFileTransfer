using Microsoft.AspNetCore.Mvc;
using SecureFileSender.Api.DTOs;
using SecureFileSender.Api.Helpers;
using SecureFileSender.Api.Data;
namespace SecureFileSender.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IConfiguration _config;
    private readonly AppDbContext _db;

    public AuthController(IConfiguration config, AppDbContext db)
    {
        _config = config;
        _db = db;
    }

	[HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        var user = _db.AdminUsers.FirstOrDefault(u => u.Email == request.Email);
        if (user != null && PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            var token = JwtTokenGenerator.GenerateToken(user.Email, _config);
            return Ok(new LoginResponseDto { Token = token });
        }

        return Unauthorized("Invalid credentials.");
    }

}
