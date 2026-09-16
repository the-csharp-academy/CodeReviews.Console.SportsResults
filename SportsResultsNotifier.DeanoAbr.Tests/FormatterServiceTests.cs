using SportsResultsNotifier.DeanoAbr.Models;
using SportsResultsNotifier.DeanoAbr.Services;

namespace SportsResultsNotifier.DeanoAbr.Tests;

public class FormatterServiceTests
{
    [Fact]
    public void BuildBody_WithGames_FormatsHeaderAndEachLine()
    {
        var svc = new FormatterService();
        var games = new List<GameResult>
        {
            new("Milwaukee Bucks", 118, "Los Angeles Lakers", 117),
            new("Boston Celtics", 101, "Miami Heat", 99),
        };

        var body = svc.BuildBody(games, new DateOnly(2026, 1, 15));

        Assert.Contains("NBA Game Results - 2026-01-15", body);
        Assert.Contains("Milwaukee Bucks 118 - 117 Los Angeles Lakers", body);
        Assert.Contains("Boston Celtics 101 - 99 Miami Heat", body);
    }

    [Fact]
    public void BuildBody_WithNoGames_ReturnsNoGamesMessage()
    {
        var svc = new FormatterService();
        var body = svc.BuildBody(Array.Empty<GameResult>(), new DateOnly(2026, 1, 15));
        Assert.Contains("No games played", body);
    }
}
