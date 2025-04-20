using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using SecureFileSender.Api.Models;
using SecureFileSender.Api.Data;
using Microsoft.EntityFrameworkCore;
using K4os.Compression.LZ4.Streams;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace SecureFileSender.Api.Controllers;

[Route("api/files")]
[Authorize]
[ApiController]
[DisableRequestSizeLimit]

public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileUploadController(IWebHostEnvironment env, AppDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

[HttpPost("upload")]
public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
{
    var sw = Stopwatch.StartNew();
    var session = new UploadSession
    {
        Id = Guid.NewGuid(),
        Status = "Processing",
        CreatedAt = DateTime.UtcNow
    };
    _db.UploadSessions.Add(session);
    await _db.SaveChangesAsync();

    var adminEmail = HttpContext.User.Identity?.Name;
    var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == adminEmail);
    if (admin == null) return Unauthorized();

    if (files == null || !files.Any())
        return BadRequest("No files uploaded.");

    

    Response.Headers["Access-Control-Expose-Headers"] = "session-id";
    Response.Headers["session-id"] = session.Id.ToString();

    try
    {
        var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        foreach (var file in files)
        {
            var originalFileName = Path.GetFileName(file.FileName);
            var compressedFileName = $"{Guid.NewGuid()}.lz4";
            var compressedPath = Path.Combine(uploadsFolder, compressedFileName);

            using var input = file.OpenReadStream();
            using var output = System.IO.File.Create(compressedPath);
            using var lz4 = LZ4Stream.Encode(output, K4os.Compression.LZ4.LZ4Level.L12_MAX);

            await input.CopyToAsync(lz4);

 var metadata = new UploadedFile
{
    OriginalFileName = originalFileName,
    CompressedFileName = compressedFileName,
    UploadedAt = DateTime.UtcNow,
    AdminUserId = admin.Id,
    SessionId = session.Id // 👈 Set it
};

            _db.UploadedFiles.Add(metadata);
            await _db.SaveChangesAsync();
        }

        session.Status = "Completed";
    }
    catch (Exception ex)
    {
        session.Status = "Failed";
        session.ErrorMessage = ex.Message;
    }

    session.UpdatedAt = DateTime.UtcNow;
    await _db.SaveChangesAsync();

    sw.Stop();
    Console.WriteLine($"[Upload] Completed in {sw.ElapsedMilliseconds} ms");

    return Ok(new { sessionId = session.Id });
}


    [HttpGet("upload-status/{id}")]
    public async Task<IActionResult> GetUploadStatus(Guid id)
    {
        var session = await _db.UploadSessions.FindAsync(id);
        if (session == null) return NotFound();

        return Ok(new { session.Status, session.ErrorMessage });
    }

    
[HttpGet("by-session/{sessionId}")]
public async Task<IActionResult> GetFilesBySession(Guid sessionId)
{
    var adminEmail = HttpContext.User.Identity?.Name;
    var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == adminEmail);
    if (admin == null) return Unauthorized();

    var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads");

    var files = await _db.UploadedFiles
        .Where(f => f.AdminUserId == admin.Id && f.SessionId == sessionId)
        .Select(f => new
        {
            f.Id,
            f.OriginalFileName,
            f.CompressedFileName,
            FullPath = Path.Combine(uploadsFolder, f.CompressedFileName), // 👈 include full path
            f.UploadedAt
        })
        .ToListAsync();

    return Ok(files);
}

}


