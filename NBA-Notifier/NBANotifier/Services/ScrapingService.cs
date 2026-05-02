using System.Net;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using NBANotifier.Interfaces;
using NBANotifier.Models;

namespace NBANotifier.Services;

public class ScrapingService(IConfiguration configuration) : IScrapingService
{
    private readonly string _nbaResultsUrl = configuration.GetValue<string>("SportsResultSiteUrls:Nba") ??
                                             throw new ArgumentNullException(nameof(configuration),
                                                 "Could not extract SportsResultSiteUrls:Nba");

    private readonly string _nbaSnapshotPath =
        Path.Combine(Utils.GetTestDataDirectory(), "basketballReference_snapshot.html");

    private readonly bool _useSnapshot = configuration.GetValue<bool>("ScrapingSettings:UseSnapshot");

    private readonly string _wnbaResultsUrl = configuration.GetValue<string>("SportsResultSiteUrls:Wnba") ??
                                              throw new ArgumentNullException(nameof(configuration),
                                                  "Could not extract SportsResultSiteUrls:Wnba");

    private readonly string _wnbaSnapshotPath =
        Path.Combine(Utils.GetTestDataDirectory(), "BasketballReference_snapshot_wnba.html");

    public void UpdateSnapshots()
    {
        HtmlWeb web = new();

        var doc = web.Load(_nbaResultsUrl);
        doc.Save(_nbaSnapshotPath);

        doc = web.Load(_wnbaResultsUrl);
        doc.Save(_wnbaSnapshotPath);
    }

    public NbaResults GetNbaResults()
    {
        var doc = new HtmlDocument();
        if (_useSnapshot)
        {
            doc.Load(_nbaSnapshotPath);
        }
        else
        {
            var web = new HtmlWeb();
            doc = web.Load(_nbaResultsUrl);
        }

        var results = new NbaResults
        {
            Games = ScrapeGames(doc),
            EasternConference = ScrapeEasternConference(doc),
            WesternConference = ScrapeWesternConference(doc)
        };

        return results;
    }

    public WnbaResults GetWnbaResults()
    {
        var doc = new HtmlDocument();
        if (_useSnapshot)
        {
            doc.Load(_wnbaSnapshotPath);
        }
        else
        {
            var web = new HtmlWeb();
            doc = web.Load(_wnbaResultsUrl);
        }

        var results = new WnbaResults
        {
            Games = ScrapeGames(doc)
        };

        return results;
    }

    //------- Helper Methods -------
    private string ExtractDecodedText(HtmlNode node, string xPath)
    {
        var raw = node.SelectSingleNode(xPath).InnerText.Trim();
        return WebUtility.HtmlDecode(raw);
    }

    private List<Game> ScrapeGames(HtmlDocument doc)
    {
        List<Game> games = [];

        var container = doc.DocumentNode.SelectSingleNode("//div[@class='game_summaries']");
        if (container is null)
            throw new InvalidOperationException("No div class with the name game_summaries was found while scraping.");

        var gameSummaries = container.SelectNodes(".//div[contains(@class, 'game_summary')]");
        if (!gameSummaries.Any()) return games;

        foreach (var game in gameSummaries)
        {
            var teamsTable = game.SelectSingleNode(".//table[@class='teams']");
            var rows = teamsTable.SelectNodes(".//tr");

            var awayTeamRow = rows[0]; //away team is always the first row, and home team is the second
            var homeTeamRow = rows[1];

            var gameResult = new Game
            {
                HomeTeam = ExtractDecodedText(homeTeamRow, ".//td[1]/a"),
                HomeScore = int.Parse(ExtractDecodedText(homeTeamRow, ".//td[@class='right']")),

                AwayTeam = ExtractDecodedText(awayTeamRow, ".//td[1]/a"),
                AwayScore = int.Parse(ExtractDecodedText(awayTeamRow, ".//td[@class='right']"))
            };

            games.Add(gameResult);
        }

        return games;
    }

    private List<Team> ScrapeEasternConference(HtmlDocument doc)
    {
        List<Team> teams = [];

        var container = doc.DocumentNode.SelectSingleNode("//div[@id='all_confs_standings_E']");
        var conferenceTable = container.SelectSingleNode(".//table[@id='confs_standings_E']");
        var teamTables = conferenceTable.SelectNodes(".//tr[@class='full_table']");

        foreach (var teamTable in teamTables)
        {
            var team = new Team
            {
                Name = ExtractDecodedText(teamTable, ".//th[@data-stat='team_name']/a"),
                Wins = ExtractDecodedText(teamTable, ".//td[@data-stat='wins']"),
                Losses = ExtractDecodedText(teamTable, ".//td[@data-stat='losses']"),
                WinLossPercentage = ExtractDecodedText(teamTable, ".//td[@data-stat='win_loss_pct']"),
                GamesBack = ExtractDecodedText(teamTable, ".//td[@data-stat='gb']"),
                PointsPerGame = ExtractDecodedText(teamTable, ".//td[@data-stat='pts_per_g']"),
                PointsAllowedPerGame = ExtractDecodedText(teamTable, ".//td[@data-stat='opp_pts_per_g']")
            };

            teams.Add(team);
        }

        return teams.OrderByDescending(t => t.Wins).ToList();
    }

    private List<Team> ScrapeWesternConference(HtmlDocument doc)
    {
        List<Team> teams = [];

        var container = doc.DocumentNode.SelectSingleNode("//div[@id='all_confs_standings_W']");
        var conferenceTable = container.SelectSingleNode(".//table[@id='confs_standings_W']");
        var teamTables = conferenceTable.SelectNodes(".//tr[@class='full_table']");

        foreach (var teamTable in teamTables)
        {
            var team = new Team
            {
                Name = ExtractDecodedText(teamTable, ".//th[@data-stat='team_name']/a"),
                Wins = ExtractDecodedText(teamTable, ".//td[@data-stat='wins']"),
                Losses = ExtractDecodedText(teamTable, ".//td[@data-stat='losses']"),
                WinLossPercentage = ExtractDecodedText(teamTable, ".//td[@data-stat='win_loss_pct']"),
                GamesBack = ExtractDecodedText(teamTable, ".//td[@data-stat='gb']"),
                PointsPerGame = ExtractDecodedText(teamTable, ".//td[@data-stat='pts_per_g']"),
                PointsAllowedPerGame = ExtractDecodedText(teamTable, ".//td[@data-stat='opp_pts_per_g']")
            };

            teams.Add(team);
        }

        return teams.OrderByDescending(t => t.Wins).ToList();
    }
}