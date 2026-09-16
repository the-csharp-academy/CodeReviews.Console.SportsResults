using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using SportsResultsNotifier.DeanoAbr.Models;

namespace SportsResultsNotifier.DeanoAbr.Services;

public class ScraperService : IScraperService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ScraperService> _logger;

    public ScraperService(IHttpClientFactory httpClientFactory, ILogger<ScraperService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<List<GameResult>> ScrapeAsync(string url, CancellationToken ct = default)
    {
        var html = await _httpClientFactory.CreateClient("BasketballReference").GetStringAsync(url, ct);
        return Parse(html);
    }

    public List<GameResult> Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var games = new List<GameResult>();
        var summaries = doc.DocumentNode.SelectNodes("//div[contains(@class,'game_summary')]");

        if (summaries is null)
        {
            _logger.LogWarning("No game summaries found on the page.");
            return games;
        }

        foreach (var summary in summaries)
        {
            var winnerRow = summary.SelectSingleNode(".//tr[contains(@class,'winner')]");
            var loserRow = summary.SelectSingleNode(".//tr[contains(@class,'loser')]");
            if (winnerRow is null || loserRow is null)
            {
                continue;
            }

            var winner = winnerRow.SelectSingleNode("./td[1]/a")?.InnerText.Trim();
            var loser = loserRow.SelectSingleNode("./td[1]/a")?.InnerText.Trim();
            var winnerScore = ParseScore(winnerRow.SelectSingleNode("./td[contains(@class,'right')]")?.InnerText);
            var loserScore = ParseScore(loserRow.SelectSingleNode("./td[contains(@class,'right')]")?.InnerText);

            if (winner is null || loser is null || winnerScore is null || loserScore is null)
            {
                continue;
            }

            games.Add(new GameResult(winner, winnerScore.Value, loser, loserScore.Value));
        }

        return games;
    }

    private static int? ParseScore(string? raw) =>
        int.TryParse(raw, out var score) ? score : null;
}
