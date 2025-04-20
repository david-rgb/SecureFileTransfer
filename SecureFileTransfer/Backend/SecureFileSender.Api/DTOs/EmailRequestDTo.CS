namespace SecureFileSender.Api.DTOs;

public class EmailRequestDto
{


    public string ToEmail { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string HtmlBody { get; set; } = default!;
}
