using System.ComponentModel.DataAnnotations;

namespace SecureFileSender.Api.Models;

public class AdminUser
{
	[Key]
	public int Id { get; set; }

	[Required]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	[Required]
	public string PasswordHash { get; set; } = string.Empty;
}
