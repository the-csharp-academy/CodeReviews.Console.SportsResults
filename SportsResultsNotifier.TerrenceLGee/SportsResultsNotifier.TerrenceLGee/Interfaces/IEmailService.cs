using SportsResultsNotifier.TerrenceLGee.Data;

namespace SportsResultsNotifier.TerrenceLGee.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailData emailData, CancellationToken token = default);
}
