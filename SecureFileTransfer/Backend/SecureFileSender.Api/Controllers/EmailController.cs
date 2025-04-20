using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;
using SecureFileSender.Api.DTOs;
using SecureFileSender.Api.Services;
namespace SecureFileSender.Api.Controllers;
using Microsoft.AspNetCore.Authorization;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly EmailService _emailService; // ✅ Add this line


    public EmailController(AppDbContext db, EmailService emailService)
    {
        _db = db;
        _emailService = emailService;

    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto dto)
    {
        var email = HttpContext.User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var admin = await _db.AdminUsers
            .Include(a => a.EmailSettings)
            .FirstOrDefaultAsync(a => a.Email == email);

        if (admin?.EmailSettings == null)
            return BadRequest("Email settings are not configured for this admin.");

        try
        {
            await _emailService.SendEmailAsync(
                settings: admin.EmailSettings,
                toEmail: dto.ToEmail,
                subject: dto.Subject,
                htmlBody: dto.HtmlBody
            );

            return Ok(new { message = "Email sent successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to send email", details = ex.Message });
        }
    }
}
