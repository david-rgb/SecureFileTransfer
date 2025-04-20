namespace SecureFileSender.Api.DTOs;

public class UpdateEmailSettingsDto
{
	public string SmtpServer { get; set; } = string.Empty;
	public int Port { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public bool UseSSL { get; set; } = true;

	public string SenderEmail { get; set; } = string.Empty;
	public string SenderDisplayName { get; set; } = "Secure File Sender";
}
