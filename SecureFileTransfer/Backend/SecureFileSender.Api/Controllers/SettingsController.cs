using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;
using SecureFileSender.Api.DTOs;
using SecureFileSender.Api.Helpers;
using SecureFileSender.Api.Models;
using Microsoft.AspNetCore.DataProtection;
namespace SecureFileSender.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class SettingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataProtector _protector;

    public SettingsController(AppDbContext db, IHttpContextAccessor httpContextAccessor, IDataProtectionProvider provider)
{
    _db = db;
    _httpContextAccessor = httpContextAccessor;
    _protector = provider.CreateProtector("EmailSettingsProtector"); // ✅
}

    private string? GetCurrentUserEmail() =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    [HttpGet("email")]
    public async Task<IActionResult> GetEmailSettings()
    {
        var email = GetCurrentUserEmail();
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var user = await _db.AdminUsers
            .Include(u => u.EmailSettings)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user?.EmailSettings == null)
            return NotFound("Email settings not configured.");

        var settings = user.EmailSettings;
       return Ok(new
{
    SmtpServer = settings.SmtpServer ?? "",
    Port = settings.Port, // int, no need for null fallback
    Username = settings.Username ?? "",
    UseSSL = settings.UseSSL, // bool, no fallback needed
    SenderEmail = settings.SenderEmail ?? "",
    SenderDisplayName = settings.SenderDisplayName ?? ""
});
    }

    [HttpPut("email")]
    public async Task<IActionResult> UpdateEmailSettings([FromBody] UpdateEmailSettingsDto dto)
    {
        var email = GetCurrentUserEmail();
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var user = await _db.AdminUsers
            .Include(u => u.EmailSettings)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null) return Unauthorized();

        if (user.EmailSettings == null)
        {
            user.EmailSettings = new EmailSettings();
            _db.EmailSettings.Add(user.EmailSettings);
        }

        user.EmailSettings.SmtpServer = dto.SmtpServer;
        user.EmailSettings.Port = dto.Port;
        user.EmailSettings.Username = dto.Username;
        user.EmailSettings.Password = _protector.Protect(dto.Password); // encryption instead of hashing
        user.EmailSettings.UseSSL = dto.UseSSL;
        user.EmailSettings.SenderEmail = dto.SenderEmail;
        user.EmailSettings.SenderDisplayName = dto.SenderDisplayName;

        await _db.SaveChangesAsync();
        return Ok("Email settings updated.");
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
    {
        var email = GetCurrentUserEmail();
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var user = await _db.AdminUsers.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return Unauthorized();

        if (!PasswordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
            return BadRequest("Current password is incorrect.");

        if (dto.NewPassword != dto.ConfirmPassword)
            return BadRequest("New passwords do not match.");

        user.PasswordHash = PasswordHasher.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync();

        return Ok("Password updated successfully.");
    }
}
