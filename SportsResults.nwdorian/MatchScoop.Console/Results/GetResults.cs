using System.Globalization;
using CSharpFunctionalExtensions;
using HtmlAgilityPack;
using MatchScoop.Console.Models;
using MatchScoop.Console.Options;
using Microsoft.Extensions.Options;

namespace MatchScoop.Console.Results;

public class GetResults
{
    private readonly ScrapingOptions _scrapingOptions;

    public GetResults(IOptions<ScrapingOptions> scrapingOptions)
    {
        _scrapingOptions = scrapingOptions.Value;
    }

    public async Task<Result<IReadOnlyList<Match>>> Handle()
    {
        var webpage = await new HtmlWeb().LoadFromWebAsync(_scrapingOptions.BaseAddress);
        var nodes = webpage.DocumentNode.SelectNodes("//*[@id=\"content\"]/div[@class='game_summaries']/div/table[@class='teams']/tbody");

        var matches = new List<Match>();

        if (nodes is null)
        {
            return matches;
        }

        foreach (var node in nodes)
        {
            var homeTeamName = node.SelectSingleNode("tr[2]/td[1]").InnerText;
            var homeTeamScore = int.Parse(node.SelectSingleNode("tr[2]/td[2]").InnerText, CultureInfo.InvariantCulture);
            var awayTeamName = node.SelectSingleNode("tr[1]/td[1]").InnerText;
            var awayTeamScore = int.Parse(node.SelectSingleNode("tr[1]/td[2]").InnerText, CultureInfo.InvariantCulture);

            var match = new Match(
                new Team(homeTeamName, homeTeamScore),
                new Team(awayTeamName, awayTeamScore)
            );

            matches.Add(match);
        }

        return matches;
    }
}
