using Microsoft.Extensions.Options;
using SportsResultsNotifier.DeanoAbr.Configuration;
using SportsResultsNotifier.DeanoAbr.Services;

namespace SportsResultsNotifier.DeanoAbr;

public class Worker : BackgroundService
{
    private readonly IScraperService _scraper;
    private readonly IEmailService _email;
    private readonly FormatterService _formatter;
    private readonly ScraperSettings _scraperSettings;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IScraperService scraper,
        IEmailService email,
        FormatterService formatter,
        IOptions<ScraperSettings> scraperSettings,
        ILogger<Worker> logger)
    {
        _scraper = scraper;
        _email = email;
        _formatter = formatter;
        _scraperSettings = scraperSettings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in daily run");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task RunOnceAsync(CancellationToken ct)
    {
        _logger.LogInformation("Scraping {Url}", _scraperSettings.Url);
        var games = await _scraper.ScrapeAsync(_scraperSettings.Url, ct);

        if (games.Count == 0)
        {
            _logger.LogWarning("No games scraped; skipping email.");
            return;
        }

        var body = _formatter.BuildBody(games, DateOnly.FromDateTime(DateTime.Now));
        await _email.SendAsync("Daily NBA Results", body, ct);
    }
}
