using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace SportsResults.JJHH17;

class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<SportsResultsService>();
        builder.Services.AddLogging();

        var app = builder.Build();
        await app.RunAsync();
    }
}

public class SportsResultsService : BackgroundService
{
    private static string TimeZoneId = ConfigurationManager.AppSettings["timeZoneId"] ?? "Europe/London";

    // Cron scheduled to run at 09:00 daily
    private static readonly CronExpression cron = CronExpression.Parse("0 9 * * *");
    // BST / GMT Timezone default
    private static readonly TimeZoneInfo Tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
    private readonly ILogger<SportsResultsService> _logger;

    public SportsResultsService(ILogger<SportsResultsService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stopToken)
    {
        _logger.LogInformation("SportsResultsService is starting.");

        while (!stopToken.IsCancellationRequested)
        {
            var next = cron.GetNextOccurrence(DateTimeOffset.UtcNow, Tz);
            if (next == null) break;

            var delay = next.Value - DateTimeOffset.UtcNow;
            _logger.LogInformation($"Next scheduled run: {next}", next.Value);

            try
            {
                await Task.Delay(delay, stopToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            try
            {
                string results = SiteScrape.GetInfo();
                await Email.SendEmail(results);
                _logger.LogInformation("Email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email.");
            }
        }

        _logger.LogInformation("SportsResultsService is stopping.");
    }
}
