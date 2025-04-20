using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;
using Microsoft.AspNetCore.DataProtection;

namespace SecureFileSender.Api.Controllers
{
	[ApiController]
	[Route("api")]
	public class DownloadController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly IDataProtector _protector;

		public DownloadController(AppDbContext context, IConfiguration configuration, IDataProtectionProvider provider)
		{
			_context = context;
			_configuration = configuration;
			_protector = provider.CreateProtector("PasscodeProtector");
		}

		[HttpGet("download/{slug}")]
public async Task<IActionResult> GetDownloadInfo(string slug, [FromQuery] string? passcode = null)
{
	var link = await _context.DownloadLinks
		.Include(l => l.Customer)
		.Include(l => l.Files)
		.FirstOrDefaultAsync(l => l.Slug.ToLower() == slug.ToLower());

	if (link == null)
		return NotFound("Download link not found.");

	if (link.ExpirationDate < DateTime.UtcNow)
		return BadRequest("Download link has expired.");

	if (link.IsPasscodeProtected)
	{
		if (string.IsNullOrWhiteSpace(passcode))
			return Unauthorized("Passcode is required.");

		var decrypted = _protector.Unprotect(link.Passcode ?? "");

		if (decrypted != passcode)
			return Unauthorized("Invalid passcode.");
	}

	var response = new
	{
		customer = new { link.Customer.Id, link.Customer.Name, link.Customer.Email },
		files = link.Files.Select(file => new
		{
			file.Id,
			file.OriginalFileName,
			file.CompressedFileName,
			Token = link.Token,
			ExpirationDate = link.ExpirationDate,
			RequiresPasscode = link.IsPasscodeProtected
		})
	};

	return Ok(response);
}

	}
}
