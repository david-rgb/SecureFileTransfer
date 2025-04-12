using System;

namespace SecureFileSender.Api.Models
{
 
    public class SharedFileLink
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string? PasscodeHash { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int DownloadCount { get; set; } = 0;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<UploadedFile> Files { get; set; } = new List<UploadedFile>();
}

}
