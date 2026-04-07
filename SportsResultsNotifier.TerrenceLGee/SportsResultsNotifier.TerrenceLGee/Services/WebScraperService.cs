using HtmlAgilityPack;
using SportsResultsNotifier.TerrenceLGee.Data;
using SportsResultsNotifier.TerrenceLGee.Interfaces;
using SportsResultsNotifier.TerrenceLGee.Models;

namespace SportsResultsNotifier.TerrenceLGee.Services;

public class WebScraperService : IWebScraperService
{
    private readonly ILogger<WebScraperService> _logger;

    public WebScraperService(ILogger<WebScraperService> logger)
    {
        _logger = logger;
    }

    public async Task<List<GameSummary>> ScrapeResultsAsync(CancellationToken token = default)
    {
        var games = new List<GameSummary>();

        var web = new HtmlWeb();

        var htmlDocument = await web
            .LoadFromWebAsync(ScrapingData.Url, token);

        if (htmlDocument is null)
        {
            throw new HttpRequestException("Unable to retrieve html document");
        }

        var gameDate = htmlDocument
            .DocumentNode
            .SelectSingleNode(ScrapingData.XPathForGameDate)
            .InnerText;

        if (gameDate is null)
        {
            throw new InvalidOperationException("Date game played on not found. " +
                "Either no games were played today or a more serious error has occurred");
        }

        var gameSummaries = htmlDocument
            .DocumentNode
            .SelectNodes(ScrapingData.XPathForGameSummaries);

        if (gameSummaries is null)
        {
            throw new InvalidOperationException("Game summaries not found, page structure possibly changed.");
        }

        for (var game = 0; game < gameSummaries.Count; game++)
        {
            token.ThrowIfCancellationRequested();

            var winner = new Team
            {
                Name = gameSummaries[game]
                .SelectSingleNode(ScrapingData.XPathForWinnerName)
                .InnerText
                .Trim(),
                FinalScore = gameSummaries[game]
                .SelectSingleNode(ScrapingData.XPathForWinnerFinalScore)
                .InnerText
                .Trim()
            };

            var loser = new Team
            {
                Name = gameSummaries[game]
                .SelectSingleNode(ScrapingData.XPathForLoserName)
                .InnerText
                .Trim(),
                FinalScore = gameSummaries[game]
                .SelectSingleNode(ScrapingData.XPathForLoserFinalScore)
                .InnerText
                .Trim()
            };

            var scoresTable = gameSummaries[game]
                .SelectSingleNode(ScrapingData.XPathForScoresTable);

            if (scoresTable is null)
            {
                _logger.LogWarning("Scores table is missing for game " +
                    "#{GameIndex}", game + 1);
                continue;
            }

            var rows = scoresTable
                .SelectNodes(ScrapingData.XPathForRowsInScoresTable);

            if (rows is null)
            {
                _logger.LogWarning("Rows table missing for game " +
                    "#{GameIndex}", game + 1);
                continue;
            }

            foreach (var row in rows)
            {
                token.ThrowIfCancellationRequested();

                var cells = row.SelectNodes(ScrapingData.XPathForCellsInRow);

                if (cells is null || cells.Count == 0)
                {
                    _logger.LogWarning("Cells are missing for the scores table in game " +
                        "#{GameIndex}", game + 1);
                    continue;
                }

                var node = cells[0]
                    .SelectSingleNode(ScrapingData.XPathForTeamNameInScoresTable);

                if (node is null)
                {
                    _logger.LogWarning("The node that contains the " +
                        "team name for the scores table in game " +
                        "#{GameIndex} is missing", game + 1);
                    continue;
                }

                var teamName = node
                    .InnerText
                    .Trim();

                if (!string.IsNullOrEmpty(winner.Name)
                    && winner.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                {
                    LoadGameScores(winner, cells);
                }
                else if (!string.IsNullOrEmpty(loser.Name) 
                    && loser.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                {
                    LoadGameScores(loser, cells);
                }
                else
                {
                    _logger.LogWarning("Team name retrieved from the scores table " +
                        "does not match the name of either the winner or the loser in game " +
                        "#{GameIndex}", game + 1);
                    continue;
                }
            }

            games.Add(new GameSummary
            {
                GameDate = gameDate,
                Winner = winner,
                Loser = loser
            });
        }
        return games;
    }

    private void LoadGameScores(
        Team team, 
        HtmlNodeCollection cells, 
        CancellationToken token = default)
    {
        for (int cell = 1; cell < cells.Count; cell++)
        {
            token.ThrowIfCancellationRequested();

            team.QuarterScores.Add(cells[cell].InnerText);
        }
    }
}
