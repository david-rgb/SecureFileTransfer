namespace SecureFileSender.Api.DTOs;

public class ShareFileDto
{
    public List<string> FilePaths { get; set; } = new();
    public string? Passcode { get; set; }
    public int ExpiresInDays { get; set; } = 7;
    public string CustomerName { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
}