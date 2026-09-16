using Microsoft.Extensions.Logging.Abstractions;
using SportsResultsNotifier.DeanoAbr.Models;
using SportsResultsNotifier.DeanoAbr.Services;

namespace SportsResultsNotifier.DeanoAbr.Tests;

public class ScraperServiceTests
{
    private const string SampleHtml = """
<div class="game_summary expanded nohover">
  <table class="teams">
    <tbody>
      <tr class="winner">
        <td><a href="/teams/MIL/">Milwaukee Bucks</a></td>
        <td class="right">118</td>
      </tr>
      <tr class="loser">
        <td><a href="/teams/LAL/">Los Angeles Lakers</a></td>
        <td class="right">117</td>
      </tr>
    </tbody>
  </table>
</div>
<div class="game_summary expanded nohover">
  <table class="teams">
    <tbody>
      <tr class="winner">
        <td><a href="/teams/BOS/">Boston Celtics</a></td>
        <td class="right">101</td>
      </tr>
      <tr class="loser">
        <td><a href="/teams/MIA/">Miami Heat</a></td>
        <td class="right">99</td>
      </tr>
    </tbody>
  </table>
</div>
""";

    [Fact]
    public void Parse_WithGameSummaries_ExtractsWinnersLosersAndScores()
    {
        var scraper = new ScraperService(null!, NullLogger<ScraperService>.Instance);

        var games = scraper.Parse(SampleHtml);

        Assert.Equal(2, games.Count);
        Assert.Equal(new GameResult("Milwaukee Bucks", 118, "Los Angeles Lakers", 117), games[0]);
        Assert.Equal(new GameResult("Boston Celtics", 101, "Miami Heat", 99), games[1]);
    }

    [Fact]
    public void Parse_WithNoSummaries_ReturnsEmptyList()
    {
        var scraper = new ScraperService(null!, NullLogger<ScraperService>.Instance);

        var games = scraper.Parse("<html><body></body></html>");

        Assert.Empty(games);
    }
}
