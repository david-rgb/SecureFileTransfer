namespace SecureFileSender.Api.Models;
public class DownloadLink
{
    public int Id { get; set; }
    public Guid Token { get; set; } = Guid.NewGuid();
    public string Slug { get; set; } = "";
    public bool IsPasscodeProtected { get; set; }
    public string? Passcode { get; set; } // Encrypted if present
    public DateTime ExpirationDate { get; set; }
    public int AdminUserId { get; set; }
    public AdminUser AdminUser { get; set; } = null!;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public List<UploadedFile> Files { get; set; } = new();
}