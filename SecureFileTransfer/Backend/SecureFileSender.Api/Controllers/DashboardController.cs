using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;

namespace SecureFileSender.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
private readonly IHttpContextAccessor _httpContextAccessor;
    public DashboardController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("shared-links")]
    public async Task<IActionResult> GetSharedLinks()
    {
        var now = DateTime.UtcNow;
        var adminEmail = HttpContext.User.Identity?.Name;
        var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == adminEmail);
        if (admin == null) return Unauthorized();

        var links = await _db.SharedFileLinks
            .Include(l => l.Customer)
            .Where(l => l.AdminUserId == admin.Id)
            .GroupBy(l => l.Token)
            .Select(g => new
            {
                Token = g.Key,
                ReceiverName = g.First().Customer.Name,
                ReceiverEmail = g.First().Customer.Email,
                DownloadCount = g.Count(), // Just a placeholder, replace if you have tracking
                Status = g.First().ExpirationDate < now ? "Expired" : "Pending",
                ExpiresIn = (g.First().ExpirationDate - now).Days,
                SentAt = g.First().ExpirationDate.AddDays(-7),
            })
            .ToListAsync();

        return Ok(links);
    }

    [HttpGet("customers")]
public async Task<IActionResult> GetCustomers()
{

       var adminEmail = HttpContext.User.Identity?.Name;
        var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == adminEmail);
        if (admin == null) return Unauthorized();
    var customers = await _db.Customers
     .Where(l => l.AdminUserId == admin.Id)
        .Select(c => new
        {
            Name = c.Name,
            Email = c.Email,
            Id = c.Id,
            DownloadCount = _db.SharedFileLinks.Count(l => l.CustomerId == c.Id && l.DownloadCount > 0)
        })
        .ToListAsync();

    return Ok(customers);
}

[HttpPost("clear-links")]
public async Task<IActionResult> ClearUploadedAndSharedLinks()
{
    _db.UploadedFiles.RemoveRange(_db.UploadedFiles);
    _db.SharedFileLinks.RemoveRange(_db.SharedFileLinks);
    await _db.SaveChangesAsync();

    return Ok("UploadedFiles and SharedFileLinks have been cleared.");
}

[HttpGet("files")]
public async Task<IActionResult> GetUploadedFiles()
{
    var adminEmail = HttpContext.User.Identity?.Name;
    var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == adminEmail);
    if (admin == null) return Unauthorized();

    var files = await _db.UploadedFiles
        .Where(f => f.AdminUserId == admin.Id)
        .Select(f => new
        {
            f.Id,
            f.OriginalFileName,
            f.CompressedFileName,
            f.UploadedAt,
            f.SessionId
        })
        .ToListAsync();

    return Ok(files);
}


[HttpGet("download-links")]
public async Task<IActionResult> GetDownloadLinks()
{
    var adminEmail = HttpContext.User.Identity?.Name;
    var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == adminEmail);
    if (admin == null) return Unauthorized();

    var links = await _db.DownloadLinks
        .Include(dl => dl.Customer)
        .Where(dl => dl.AdminUserId == admin.Id)
        .Select(dl => new
        {
            Id = dl.Id,
            Slug = dl.Slug,
            Token = dl.Token,
            CustomerName = dl.Customer.Name,
            CustomerEmail = dl.Customer.Email,
            FileCount = dl.Files.Count,
            IsPasscodeProtected = dl.IsPasscodeProtected,
            ExpiresAt = dl.ExpirationDate
        })
        .ToListAsync();

    return Ok(links);
}

}
