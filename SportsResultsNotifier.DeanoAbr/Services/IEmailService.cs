namespace SportsResultsNotifier.DeanoAbr.Services;

public interface IEmailService
{
    Task SendAsync(string subject, string body, CancellationToken ct = default);
}
