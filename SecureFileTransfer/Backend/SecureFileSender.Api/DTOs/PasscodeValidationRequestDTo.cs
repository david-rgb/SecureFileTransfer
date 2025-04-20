namespace SecureFileSender.Api.DTOs;
public class PasscodeValidationDto
{
	public string Slug { get; set; } = string.Empty;
	public string Passcode { get; set; } = string.Empty;
}