using MailKit.Net.Smtp;

namespace SportsResultsNotifier.TerrenceLGee.Interfaces;

public interface ISmtpClientFactory
{
    ISmtpClient Create();
}
