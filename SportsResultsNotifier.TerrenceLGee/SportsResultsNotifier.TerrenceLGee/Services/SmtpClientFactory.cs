using MailKit.Net.Smtp;
using SportsResultsNotifier.TerrenceLGee.Interfaces;

namespace SportsResultsNotifier.TerrenceLGee.Services;

public class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient Create()
    {
        return new SmtpClient();
    }
}
