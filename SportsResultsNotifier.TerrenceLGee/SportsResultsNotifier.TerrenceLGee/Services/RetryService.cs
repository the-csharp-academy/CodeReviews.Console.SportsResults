using SportsResultsNotifier.TerrenceLGee.Interfaces;

namespace SportsResultsNotifier.TerrenceLGee.Services;

public class RetryService : IRetryService
{
    private readonly ILogger<RetryService> _logger;

    public RetryService(ILogger<RetryService> logger)
    {
        _logger = logger;
    }

    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation, 
        int maxRetries, 
        TimeSpan delay, 
        Func<Exception, bool>? shouldRetry = null, 
        CancellationToken token = default)
    {
        if (maxRetries < 1)
        {
            ArgumentOutOfRangeException
                .ThrowIfLessThan(maxRetries, 1);
        }

        Exception? lastException = null;

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await operation(token);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex,
                    "Attempt {Attempt} of {MaxRetries} failed",
                    attempt,
                    maxRetries);

                if (shouldRetry is not null && !shouldRetry(ex)) throw;

                if (attempt < maxRetries)
                {
                    var expBackoff = TimeSpan.FromMilliseconds(
                        delay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    await Task.Delay(expBackoff, token);
                    continue;
                }

                lastException = ex;
            }
        }

        _logger.LogError(lastException,
            "Operation failed after {MaxRetries} attempts",
            maxRetries);

        throw lastException!;
    }

    public async Task ExecuteAsync(
        Func<CancellationToken, Task> operation, 
        int maxRetries, 
        TimeSpan delay, 
        Func<Exception, bool>? shouldRetry = null, 
        CancellationToken token = default)
    {
        await ExecuteAsync(async token =>
        {
            await operation(token);
            return true;
        }, maxRetries, delay, shouldRetry, token);
    }
}
