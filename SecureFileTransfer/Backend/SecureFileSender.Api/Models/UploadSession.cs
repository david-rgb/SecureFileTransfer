namespace SecureFileSender.Api.Models;

public class UploadSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Status { get; set; } = "Uploading"; // Uploading, Processing, Done, Failed
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // ✅ Add this line

}