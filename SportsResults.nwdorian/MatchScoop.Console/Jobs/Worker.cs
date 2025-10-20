using MatchScoop.Console.Email;
using MatchScoop.Console.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MatchScoop.Console.Jobs;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly GetResults _getResults;
    private readonly SendResultsEmail _sendResultsEmail;

    public Worker(ILogger<Worker> logger, GetResults getResults, SendResultsEmail sendResultsEmail)
    {
        _logger = logger;
        _getResults = getResults;
        _sendResultsEmail = sendResultsEmail;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting background job.");

        using var timer = new PeriodicTimer(TimeSpan.FromDays(1));

        try
        {
            do
            {
                _logger.LogInformation("Getting match information.");
                var result = await _getResults.Handle();
                if (result.IsFailure)
                {
                    _logger.LogInformation("There was an error getting results: {Error}", result.Error);
                    return;
                }
                var matches = result.Value;
                _logger.LogInformation("{GamesCount} games gathered.", matches.Count);

                _logger.LogInformation("Sending email.");
                await _sendResultsEmail.Handle(matches);
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException oex)
        {
            _logger.LogError(oex, "Stopping background job.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error.");
        }
    }
}
