using System.ComponentModel.DataAnnotations;

namespace SecureFileSender.Api.Models;

public class EmailSettings
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string SmtpServer { get; set; } = string.Empty;

	[Required]
	public int Port { get; set; }

	[Required]
	public string Username { get; set; } = string.Empty;

	[Required]
	public string Password { get; set; } = string.Empty;

	public bool UseSSL { get; set; } = true;

	[Required]
	public string SenderEmail { get; set; } = string.Empty;

	public string SenderDisplayName { get; set; } = "Secure File Sender";

	// Optional FK to AdminUser if you want multi-tenant config
	public int? AdminUserId { get; set; }
}
