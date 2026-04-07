using SportsResultsNotifier.TerrenceLGee.Models;

namespace SportsResultsNotifier.TerrenceLGee.Interfaces;

public interface IWebScraperService
{
    Task<List<GameSummary>> ScrapeResultsAsync(CancellationToken token = default);
}
