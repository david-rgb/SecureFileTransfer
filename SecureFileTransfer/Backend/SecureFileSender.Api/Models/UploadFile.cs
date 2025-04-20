namespace SecureFileSender.Api.Models;

public class UploadedFile
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = default!;
    public string CompressedFileName { get; set; } = default!;
    public DateTime UploadedAt { get; set; }
        public int DownloadCount { get; set; } = 0;
    public int AdminUserId { get; set; }
public AdminUser AdminUser { get; set; } = null!;
    public Guid SessionId { get; set; } // 👈 Add this

}
