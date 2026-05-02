using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NBANotifier.Interfaces;
using NBANotifier.Models;

namespace NBANotifier.Services;

public class EmailService(EmailSettings emailSettings) : IEmailService
{
    public MimeMessage BuildEmail(string htmlBody, Subscriber subscriber)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings.SenderName, emailSettings.SenderEmail));
        message.To.Add(new MailboxAddress(subscriber.Name, subscriber.Email));
        message.Subject = $"Daily NBA Report - {DateTime.Today:yyyy-M-d dddd}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = "Please view this email through a client that supports HTML."
        };

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }

    public async Task SendEmailAsync(MimeMessage message, Subscriber subscriber)
    {
        var socketOptions = emailSettings.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        using var client = new SmtpClient();
        await client.ConnectAsync(emailSettings.SmtpHost, emailSettings.SmtpPort, socketOptions);
        if (!string.IsNullOrEmpty(emailSettings.SmtpUser) && !string.IsNullOrEmpty(emailSettings.SmtpPassword))
            await client.AuthenticateAsync(emailSettings.SmtpUser, emailSettings.SmtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}