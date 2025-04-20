namespace SecureFileSender.Api.DTOs;
using System.Text.Json.Serialization;
public class UpdatePasswordDto
{
    [JsonPropertyName("currentPassword")]
    public string CurrentPassword { get; set; }= string.Empty;

    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; }= string.Empty;

    [JsonPropertyName("confirmPassword")]
    public string ConfirmPassword { get; set; }= string.Empty;
}

