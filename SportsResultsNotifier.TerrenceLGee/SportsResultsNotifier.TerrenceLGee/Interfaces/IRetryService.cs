namespace SportsResultsNotifier.TerrenceLGee.Interfaces;

public interface IRetryService
{
    Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation, 
        int maxRetries, 
        TimeSpan delay, 
        Func<Exception, bool>? shouldRetry = null,
        CancellationToken token = default);

    Task ExecuteAsync(
        Func<CancellationToken, Task> operation, 
        int maxRetries, 
        TimeSpan delay, 
        Func<Exception, bool>? shouldRetry = null,
        CancellationToken token = default);
}
