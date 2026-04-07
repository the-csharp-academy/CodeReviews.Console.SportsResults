using Microsoft.Extensions.Options;
using MimeKit;
using SportsResultsNotifier.TerrenceLGee.Data;
using SportsResultsNotifier.TerrenceLGee.Interfaces;

namespace SportsResultsNotifier.TerrenceLGee.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _configuration;
    private readonly ISmtpClientFactory _smtpClientFactory;

    public EmailService(
        IOptions<EmailConfiguration> configuration,
        ISmtpClientFactory smtpClientFactory)
    {
        _configuration = configuration.Value;
        _smtpClientFactory = smtpClientFactory;
    }

    public async Task SendEmailAsync(EmailData emailData, CancellationToken token = default)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(_configuration.SenderName, _configuration.SenderEmail));
        email.To.Add(new MailboxAddress(_configuration.ReceiverName, _configuration.ReceiverEmail));

        email.Subject = emailData.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = emailData.Body
        };

        email.Body = builder.ToMessageBody();

        using (var smtp = _smtpClientFactory.Create())
        {
            await smtp.ConnectAsync(_configuration.Host, _configuration.Port, false, token);

            await smtp.AuthenticateAsync(_configuration.SenderEmail, _configuration.Password, token);

            await smtp.SendAsync(email, token);

            await smtp.DisconnectAsync(true, token);
        }
    }
}
