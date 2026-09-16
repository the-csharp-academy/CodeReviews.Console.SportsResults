using SportsResultsNotifier.DeanoAbr.Models;

namespace SportsResultsNotifier.DeanoAbr.Services;

public interface IScraperService
{
    Task<List<GameResult>> ScrapeAsync(string url, CancellationToken ct = default);
}
