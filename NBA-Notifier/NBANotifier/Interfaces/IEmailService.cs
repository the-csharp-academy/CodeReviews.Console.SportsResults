using MimeKit;
using NBANotifier.Models;

namespace NBANotifier.Interfaces;

public interface IEmailService
{
    public MimeMessage BuildEmail(string htmlBody, Subscriber subscriber);
    public Task SendEmailAsync(MimeMessage message, Subscriber subscriber);
}