using System;

namespace SecureFileSender.Api.Models
{
 
    public class SharedFileLink
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    	public string? Passcode { get; set; } // encrypted
	public bool RequiresPasscode { get; set; } = false;
    public DateTime ExpirationDate { get; set; }
    public int DownloadCount { get; set; } = 0;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public ICollection<UploadedFile> Files { get; set; } = new List<UploadedFile>();
    public int AdminUserId { get; set; }
public AdminUser AdminUser { get; set; } = null!;
}
}
