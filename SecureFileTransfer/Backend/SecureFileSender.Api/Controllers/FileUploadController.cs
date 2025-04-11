using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using SecureFileSender.Api.Models;
using SecureFileSender.Api.Data;

namespace SecureFileSender.Api.Controllers;

[Route("api/files")]
[ApiController]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDbContext _db;

    public FileUploadController(IWebHostEnvironment env, AppDbContext db)
    {
        _env = env;
        _db = db;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
    {
        if (files == null || !files.Any())
            return BadRequest("No files uploaded.");

        var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var results = new List<object>();

        foreach (var file in files)
        {
            var originalFileName = Path.GetFileName(file.FileName);
            var compressedFileName = $"{Guid.NewGuid()}.gz";
            var compressedPath = Path.Combine(uploadsFolder, compressedFileName);

            // Compress and save the file
            using (var originalStream = file.OpenReadStream())
            using (var compressedFileStream = new FileStream(compressedPath, FileMode.Create))
            using (var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
            {
                await originalStream.CopyToAsync(compressionStream);
            }

            // Save metadata
            var metadata = new UploadedFile
            {
                OriginalFileName = originalFileName,
                CompressedFileName = compressedFileName,
                UploadedAt = DateTime.UtcNow
            };

            _db.UploadedFiles.Add(metadata);
            await _db.SaveChangesAsync();

            results.Add(new
            {
                id = metadata.Id,
                path = compressedPath // Full path for sharing
            });
        }

        return Ok(new { files = results });
    }
}
