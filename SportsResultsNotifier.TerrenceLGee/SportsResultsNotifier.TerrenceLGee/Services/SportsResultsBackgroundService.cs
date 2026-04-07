using MailKit.Net.Smtp;
using SportsResultsNotifier.TerrenceLGee.Data;
using SportsResultsNotifier.TerrenceLGee.Interfaces;

namespace SportsResultsNotifier.TerrenceLGee.Services;

public class SportsResultsBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRetryService _retryService;
    private readonly ILogger<SportsResultsBackgroundService> _logger;

    public SportsResultsBackgroundService(
        IServiceScopeFactory scopeFactory,
        IRetryService retryService,
        ILogger<SportsResultsBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _retryService = retryService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Sports Results Service running at: {time}", DateTime.Now);
            }

            try
            {
                var today = DateTime.Now;
                var actualRunTime = new DateTime(today.Year, today.Month, today.Day, 9, 0, 0);

                var nextRun = new DateTime();

                if (today > actualRunTime)
                {
                    nextRun = actualRunTime.AddDays(1);
                }
                else
                {
                    nextRun = actualRunTime;
                }

                var delay = nextRun - today;

                await Task.Delay(delay, stoppingToken);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var webScraperService = scope.ServiceProvider.GetRequiredService<IWebScraperService>();
                    var formatterService = scope.ServiceProvider.GetRequiredService<IFormatterService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var retries = 3;

                    _logger.LogInformation("Retrieving game results at {time}", DateTime.Now);

                    var games = await _retryService.ExecuteAsync(async token => await
                    webScraperService.ScrapeResultsAsync(stoppingToken),
                    retries,
                    TimeSpan.FromMilliseconds(1000),
                    ex => ex is HttpRequestException,
                    stoppingToken);

                    var formattedEmailString = formatterService.FormatGameResults(games);

                    var emailData = new EmailData
                    {
                        Subject = "Today's basketball game results",
                        Body = formattedEmailString
                    };

                    _logger.LogInformation("Sending email at {time}", DateTime.Now);

                    await _retryService.ExecuteAsync(async token =>
                    emailService.SendEmailAsync(emailData, stoppingToken),
                    retries,
                    TimeSpan.FromMilliseconds(1000),
                    ex => ex is SmtpProtocolException,
                    stoppingToken);
                }
            }
            catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
            {
                var serviceName = $"{nameof(SportsResultsBackgroundService)}";
                _logger.LogInformation(ex, "Background service {ServiceName} " +
                    "is stopping: {msg}", serviceName, ex.Message);
            }
        }
    }
}
