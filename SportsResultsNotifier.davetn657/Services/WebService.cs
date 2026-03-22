using HtmlAgilityPack;
using SportsResultsNotifier.davetn657.Data;

namespace SportsResultsNotifier.davetn657.Services;

public interface IWebService
{
    public string GetTitle();
    public List<GameData> GetAllGameData();
    public string GetHtmlDate();
}

public class WebService : IWebService
{
    private readonly HtmlWeb _htmlWeb;
    private const string _html = @"https://www.basketball-reference.com/boxscores/";
    public WebService(HtmlWeb htmlWeb)
    {
        _htmlWeb = htmlWeb;
    }

    public string GetTitle()
    {
        try
        {
            var htmlDoc = _htmlWeb.Load(_html + GetHtmlDate());

            var node = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"content\"]/h1").First();
            Console.WriteLine(node.InnerText);
            return node.InnerText;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not reach website");
            Console.WriteLine($"Error: {ex.Message}");
            return string.Empty;
        }
    }

    public List<GameData> GetAllGameData()
    {
        var htmlDoc = _htmlWeb.Load(_html + GetHtmlDate());

        var nodes = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"content\"]/div[3]//div").ToList();
        var games = new List<GameData>();

        try
        {
            foreach (var node in nodes)
            {
                var gameData = new GameData
                {
                    Winner = node.SelectSingleNode(".//tr[1]//a").InnerText,
                    Loser = node.SelectSingleNode(".//tr[2]//a").InnerText,
                    WinnerScore = node.SelectSingleNode(".//tr[1]//td[2]").InnerText,
                    LoserScore = node.SelectSingleNode(".//tr[2]//td[2]").InnerText,
                    WinnerRounds = node.SelectNodes("//table[2]/tbody/tr[1]/td[@class='center']").Select(node => node.InnerText).ToList(),
                    LoserRounds = node.SelectNodes("//table[2]/tbody/tr[2]/td[@class='center']").Select(node => node.InnerText).ToList()

                };

                games.Add(gameData);
                Console.WriteLine($"Added: {gameData.Winner} vs {gameData.Loser}");
            }

        }
        catch
        {
            Console.WriteLine("Could not retrive game data");
        }
        return games;
    }

    public string GetHtmlDate()
    {
        var today = DateTime.Now.AddDays(-1);
        var format = $"?month={today.Month}&day={today.Day}&year={today.Year}";

        return format;
    }
}