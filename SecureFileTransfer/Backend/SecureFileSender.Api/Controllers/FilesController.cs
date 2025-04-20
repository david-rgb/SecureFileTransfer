using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;
using SecureFileSender.Api.Models;
using SecureFileSender.Api.DTOs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;

namespace SecureFileSender.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _protector;

        public FilesController(AppDbContext db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IDataProtectionProvider provider)
{
    _db = db;
    _configuration = configuration;
    _httpContextAccessor = httpContextAccessor;
    _protector = provider.CreateProtector("PasscodeProtector");
}
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteFile(int id)
{
	var file = await _db.UploadedFiles.FindAsync(id);
	if (file == null) return NotFound();
	_db.UploadedFiles.Remove(file);
	await _db.SaveChangesAsync();
	return Ok();
}

[Authorize]
[HttpDelete("download-link/{id}")]
public async Task<IActionResult> DeleteDownloadLink(int id)
{
	var email = HttpContext.User.Identity?.Name;
	if (string.IsNullOrWhiteSpace(email))
		return Unauthorized();

	var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == email);
	if (admin == null)
		return Unauthorized();

	var link = await _db.DownloadLinks
		.Include(dl => dl.Files) // optional, if you want to inspect files
		.FirstOrDefaultAsync(dl => dl.Id == id && dl.AdminUserId == admin.Id);

	if (link == null)
		return NotFound("Download link not found or access denied.");

	_db.DownloadLinks.Remove(link);
	await _db.SaveChangesAsync();

	return Ok(new { message = "Download link deleted." });
}



  [HttpPost("share")]
public async Task<IActionResult> ShareFile([FromBody] ShareFileDto request)
{
	if (request.FilePaths == null || !request.FilePaths.Any())
		return BadRequest("No file paths provided.");

	var missing = request.FilePaths.Where(path => !System.IO.File.Exists(path)).ToList();
	if (missing.Any())
		return BadRequest($"These files were not found: {string.Join(", ", missing)}");

	var adminEmail = HttpContext.User.Identity?.Name;
	var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == adminEmail);
	if (admin == null) return Unauthorized();

	// 🧠 Get or create customer
	var customer = await _db.Customers
		.FirstOrDefaultAsync(c => c.Email.ToLower() == request.CustomerEmail.ToLower());

	if (customer == null)
	{
		customer = new Customer
		{
			Name = request.CustomerName,
			Email = request.CustomerEmail,
			LastName = request.CustomerLastName,
			AdminUserId = admin.Id
		};
		_db.Customers.Add(customer);
		await _db.SaveChangesAsync();
	}

	// 🧠 Generate or use custom slug
	string baseSlug = string.IsNullOrWhiteSpace(request.Slug)
		? $"{request.CustomerName.ToLower().Replace(" ", "-")}-{customer.Id}"
		: request.Slug.ToLower().Replace(" ", "-");

	// 🛡️ Ensure slug uniqueness
	int suffix = 1;
	string finalSlug = baseSlug;
	while (await _db.DownloadLinks.AnyAsync(dl => dl.Slug == finalSlug))
	{
		finalSlug = $"{baseSlug}-{suffix++}";
	}

	var encryptedPasscode = string.IsNullOrWhiteSpace(request.Passcode)
		? null
		: _protector.Protect(request.Passcode);

	var downloadLink = new DownloadLink
	{
		Slug = finalSlug,
		IsPasscodeProtected = !string.IsNullOrWhiteSpace(request.Passcode),
		Passcode = encryptedPasscode,
		ExpirationDate = DateTime.UtcNow.AddDays(request.ExpiresInDays),
		AdminUserId = admin.Id,
		CustomerId = customer.Id
	};

	foreach (var filePath in request.FilePaths)
	{
		var compressedName = Path.GetFileName(filePath);
		var uploadedFile = await _db.UploadedFiles
			.FirstOrDefaultAsync(f => f.CompressedFileName == compressedName);

		if (uploadedFile != null)
		{
			downloadLink.Files.Add(uploadedFile);
		}
	}

	_db.DownloadLinks.Add(downloadLink);
	await _db.SaveChangesAsync();

	var frontendBase = _configuration["FrontendBaseUrl"];
	var linkUrl = $"{Request.Scheme}://{frontendBase}/download/{finalSlug}";

	return Ok(new { Link = linkUrl });
}


[HttpPost("download")]
public async Task<IActionResult> DownloadFile([FromBody] DownloadRequestDto request)
{
	if (request.FileId <= 0)
		return BadRequest("Invalid file ID.");

	var link = await _db.DownloadLinks
		.Include(l => l.Files)
		.FirstOrDefaultAsync(l => l.Files.Any(f => f.Id == request.FileId));

	if (link == null)
		return NotFound("Download link not found.");

	if (link.ExpirationDate < DateTime.UtcNow)
		return BadRequest("Download link has expired.");

	var file = link.Files.First(f => f.Id == request.FileId);
	var fullPath = Path.Combine("uploads", file.CompressedFileName);

	if (!System.IO.File.Exists(fullPath))
		return NotFound("File not found on server.");

	file.DownloadCount++;
await _db.SaveChangesAsync();

	var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
	return File(stream, "application/octet-stream", file.OriginalFileName);
}





[HttpGet("link/{slug}")]
[AllowAnonymous]
public async Task<IActionResult> GetLinkInfo(string slug)
{
	if (string.IsNullOrWhiteSpace(slug))
		return BadRequest("Slug is required.");

	var link = await _db.DownloadLinks
		.Include(dl => dl.Files)
		.Include(dl => dl.Customer)
		.FirstOrDefaultAsync(dl => dl.Slug == slug.ToLower());

	if (link == null)
		return NotFound("Download link not found.");

	// Check if link is expired
	if (link.ExpirationDate < DateTime.UtcNow)
		return BadRequest("This link has expired.");

	return Ok(new
	{
		customer = new
		{
			name = link.Customer.Name,
			email = link.Customer.Email
		},
		files = link.Files.Select(file => new
		{
			id = file.Id,
			originalFileName = file.OriginalFileName,
			compressedFileName = file.CompressedFileName
		}).ToList(),
		expiresAt = link.ExpirationDate,
		requiresPasscode = link.IsPasscodeProtected
	});
}

}}