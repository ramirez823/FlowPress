using FlowPress.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace FlowPress.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var host = _config["EmailSettings:SmtpHost"];
        var port = int.Parse(_config["EmailSettings:SmtpPort"]!);
        var senderEmail = _config["EmailSettings:SenderEmail"];
        var senderName = _config["EmailSettings:SenderName"];
        var appPassword = _config["EmailSettings:AppPassword"];

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(senderEmail, appPassword),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(senderEmail!, senderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);
        await client.SendMailAsync(message);
    }
}