namespace SecureFileSender.Api.Models;

public class UploadedFile
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = default!;
    public string CompressedFileName { get; set; } = default!;
    public DateTime UploadedAt { get; set; }
}
