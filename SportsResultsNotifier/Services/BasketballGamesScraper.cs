using HtmlAgilityPack;
using SportsResultsNotifier.Models.DTO;

namespace SportsResultsNotifier.Services;

internal class BasketballGamesScraper
{
    private string Url
    {
        get
        {
            return $"https://www.basketball-reference.com/boxscores/?month={DataDate.Month}&day={DataDate.Day}&year={DataDate.Year}";
        }
    }

    public DateOnly DataDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public ICollection<MatchSummaryDTO>? CollectData()
    {
        var web = new HtmlWeb();
        var document = web.Load(Url);

        // For local testing
        //var document = new HtmlDocument();
        //document.LoadHtml(TestHTML.HTMLstring);

        var gamesNodes = document.DocumentNode.SelectNodes("//*[@id=\"content\"]//*[contains(concat(' ', normalize-space(@class), ' '), ' game_summary ')]");

        if (gamesNodes == null || gamesNodes!.Count == 0)
            return null;

        var matchesList = new List<MatchSummaryDTO>();
        foreach (var gameNode in gamesNodes!)
            matchesList.Add(MapNodeToDTO(gameNode));

        return matchesList;
    }

    public MatchSummaryDTO MapNodeToDTO(HtmlNode htmlNode)
    {
        var match = new MatchSummaryDTO();
        match.WinnerName = htmlNode.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' winner ')]/td[1]")?.InnerText;
        match.WinnerTotalPoints = htmlNode.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' winner ')]/td[2]")?.InnerText;
        
        match.LoserName = htmlNode.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' loser ')]/td[1]")?.InnerText;
        match.LoserTotalPoints = htmlNode.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' loser ')]/td[2]")?.InnerText;
        
        match.WinnerQuarterPoints = htmlNode.SelectNodes(".//table[2]/tbody/tr[1]/td")?.Skip(1).Take(4).Select(node => node.InnerText).ToArray();
        match.LoserQuarterPoints = htmlNode.SelectNodes(".//table[2]/tbody/tr[2]/td")?.Skip(1).Take(4).Select(node => node.InnerText).ToArray();

        match.PtsPlayerName = htmlNode.SelectSingleNode(".//*[@class=\"stats\"]//tr[1]/td[2]/a")?.InnerText;
        match.PtsPoints = htmlNode.SelectSingleNode(".//*[@class=\"stats\"]//tr[1]/td[@class=\"right\"]")?.InnerText;

        match.TrbPlayerName = htmlNode.SelectSingleNode(".//*[@class=\"stats\"]//tr[2]/td[2]/a")?.InnerText;
        match.TrbPoints = htmlNode.SelectSingleNode(".//*[@class=\"stats\"]//tr[2]/td[@class=\"right\"]")?.InnerText;

        return match;
    }
}

