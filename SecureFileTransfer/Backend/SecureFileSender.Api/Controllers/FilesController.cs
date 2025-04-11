using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;
using SecureFileSender.Api.Models;
using System.Security.Cryptography;
using System.Text;
using SecureFileSender.Api.DTOs;
namespace SecureFileSender.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public FilesController(AppDbContext db)
        {
            _db = db;
        }


        [HttpPost("share")]
public async Task<IActionResult> ShareFile([FromBody] ShareFileDto request)
{
    if (request.FilePaths == null || !request.FilePaths.Any())
        return BadRequest("No file paths provided.");

    var missing = request.FilePaths.Where(path => !System.IO.File.Exists(path)).ToList();
    if (missing.Any())
        return NotFound($"These files were not found: {string.Join(", ", missing)}");

    var token = Guid.NewGuid().ToString();
    var passcodeHash = string.IsNullOrWhiteSpace(request.Passcode)
        ? null
        : Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.Passcode)));

    var customer = await _db.Customers
        .FirstOrDefaultAsync(c => c.Email.ToLower() == request.CustomerEmail.ToLower());

    if (customer == null)
    {
        customer = new Customer
        {
            Name = request.CustomerName,
            Email = request.CustomerEmail
        };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
    }

    // Create a shared file link per file
    foreach (var filePath in request.FilePaths)
    {
        var link = new SharedFileLink
        {
            Token = token,
            FilePath = filePath,
            PasscodeHash = passcodeHash,
            ExpirationDate = DateTime.UtcNow.AddDays(request.ExpiresInDays),
            CustomerId = customer.Id
        };

        _db.SharedFileLinks.Add(link);
    }

    await _db.SaveChangesAsync();

    var customerSlug = $"{customer.Name.ToLower().Replace(" ", "-")}-{token}";
    var linkUrl = $"{Request.Scheme}://{Request.Host}/download/{customerSlug}";
    return Ok(new { Link = linkUrl });
}


        public class DownloadRequestDto
{
    public string Token { get; set; } = null!;
    public string? Passcode { get; set; }
    public string FileName { get; set; } = null!;
}

    [HttpPost("download")]
public async Task<IActionResult> DownloadFile([FromBody] DownloadRequestDto request)
{
    Console.WriteLine($"Download request: token={request.Token}, fileName={request.FileName}");

    if (string.IsNullOrWhiteSpace(request.FileName))
        return BadRequest("Filename is required.");

    // Pull all links for this token
    var links = await _db.SharedFileLinks
        .Where(l => l.Token == request.Token)
        .ToListAsync();

    // Now filter by filename in memory
    var link = links.FirstOrDefault(l => Path.GetFileName(l.FilePath) == request.FileName);

    if (link == null)
        return NotFound("Download link not found.");

    if (link.ExpirationDate < DateTime.UtcNow)
        return BadRequest("Download link has expired.");

    if (link.PasscodeHash != null)
    {
        if (string.IsNullOrWhiteSpace(request.Passcode))
            return Unauthorized("Passcode required.");

        var passcodeHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.Passcode)));
        if (passcodeHash != link.PasscodeHash)
            return Unauthorized("Invalid passcode.");
    }

    if (!System.IO.File.Exists(link.FilePath))
        return NotFound("File not found on server.");
Console.WriteLine($"Found {links.Count} links for token.");
foreach (var l in links)
{
    Console.WriteLine($"Link path: {l.FilePath}");
}
    var stream = new FileStream(link.FilePath, FileMode.Open, FileAccess.Read);
    var fileName = Path.GetFileName(link.FilePath);
    return File(stream, "application/octet-stream", fileName);
}




        [HttpGet("link/{slug}")]
public async Task<IActionResult> GetLinkInfo(string slug)
{
    if (string.IsNullOrWhiteSpace(slug))
        return BadRequest("Invalid slug format.");

    var parts = slug.Split('-');
    if (parts.Length < 5)
        return BadRequest("Invalid slug format.");

    // Try to extract the last 5 parts and rebuild them as a GUID
    var maybeGuid = string.Join("-", parts.Skip(parts.Length - 5));

    if (!Guid.TryParse(maybeGuid, out var tokenGuid))
        return BadRequest("Invalid token in slug.");

    var token = tokenGuid.ToString();

    var link = await _db.SharedFileLinks.FirstOrDefaultAsync(l => l.Token == token);
    if (link == null)
        return NotFound("Download link not found.");


    var files = await _db.SharedFileLinks
    .Where(l => l.Token == token)
    .ToListAsync();

    return Ok(new
    {
        fileNames = files.Select(f => Path.GetFileName(f.FilePath)).ToList(),
        downloadUrl = "/api/files/download",
        token = token,
        requiresPasscode = link.PasscodeHash != null,
        expiresAt = link.ExpirationDate
    });
}

    }
}
