namespace SecureFileSender.Api.DTOs;
using System.ComponentModel.DataAnnotations;
public class LoginRequestDto
{
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	public string Password { get; set; } = string.Empty;
}
