using System.Net;
using System.Net.Mail;
using SecureFileSender.Api.Models;
using Microsoft.AspNetCore.DataProtection;

namespace SecureFileSender.Api.Services;

public class EmailService
{
    private readonly IDataProtector _protector;

    public EmailService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("EmailSettingsProtector"); // Only set once here
    }

    public async Task SendEmailAsync(EmailSettings settings, string toEmail, string subject, string htmlBody)
    {
        var decryptedPassword = _protector.Unprotect(settings.Password);

        var client = new SmtpClient(settings.SmtpServer, settings.Port)
        {
            Credentials = new NetworkCredential(settings.Username, decryptedPassword),
            EnableSsl = settings.UseSSL,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        var mail = new MailMessage
        {
            From = new MailAddress(settings.SenderEmail, settings.SenderDisplayName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mail.To.Add(toEmail);
        await client.SendMailAsync(mail);
    }
}
