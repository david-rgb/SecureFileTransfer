using Microsoft.AspNetCore.Mvc;
using SecureFileSender.Api.DTOs;
using SecureFileSender.Api.Helpers;
using SecureFileSender.Api.Data;
namespace SecureFileSender.Api.Controllers;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Models;
using Microsoft.AspNetCore.DataProtection;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IConfiguration _config;
    private readonly AppDbContext _db;
    private readonly IDataProtector _protector;


    public AuthController(AppDbContext db, IConfiguration config, IDataProtectionProvider provider)
{
    _db = db;
    _config = config;
    _protector = provider.CreateProtector("PasscodeProtector");
}

	[HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        var user = _db.AdminUsers.FirstOrDefault(u => u.Email.ToLower() == request.Email.ToLower());
        if (user != null && PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            var token = JwtTokenGenerator.GenerateToken(user.Email, _config);
            return Ok(new LoginResponseDto { Token = token });
        }

        return Unauthorized("Invalid credentials.");
    }

    [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto dto)
{
    if (await _db.AdminUsers.AnyAsync(u => u.Email == dto.Email))
        return BadRequest("User with this email already exists.");

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

    var user = new AdminUser
    {
        Email = dto.Email,
        PasswordHash = hashedPassword
    };

    _db.AdminUsers.Add(user);
    await _db.SaveChangesAsync();

    return Ok("Registration successful.");
}
[HttpPost("validate-passcode")]
public async Task<IActionResult> ValidatePasscode([FromBody] PasscodeValidationDto dto)
{
    if (string.IsNullOrWhiteSpace(dto.Slug))
        return BadRequest("Slug is required.");

    var link = await _db.DownloadLinks.FirstOrDefaultAsync(dl => dl.Slug == dto.Slug.ToLower());

    if (link == null)
        return NotFound("Download link not found.");

    if (!link.IsPasscodeProtected)
        return Ok(new { valid = true });

    if (string.IsNullOrWhiteSpace(dto.Passcode))
        return BadRequest("Passcode is required.");

    try
    {
        var decrypted = _protector.Unprotect(link.Passcode ?? "");
        if (dto.Passcode != decrypted)
            return Unauthorized("Incorrect passcode.");
    }
    catch
    {
        return Unauthorized("Invalid or corrupted passcode.");
    }

    return Ok(new { valid = true });
}



}
