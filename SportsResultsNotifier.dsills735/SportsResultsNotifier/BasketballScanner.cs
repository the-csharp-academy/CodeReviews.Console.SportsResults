using HtmlAgilityPack;

namespace SportsResultsNotifier;

internal class BasketballScanner
{
    public static void Scan()
    {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument document = web.Load("https://www.basketball-reference.com/boxscores/");

        var games = document.DocumentNode.SelectNodes("//div[contains(@class, 'game_summary')]");
        List<Game> gameList = new List<Game>();

        if (games == null)
            return;

        foreach (var game in games)
        {
            var rows = game.SelectNodes(".//tr");

            if (rows == null || rows.Count < 2)
                continue;

            var team1Node = game.SelectSingleNode(".//tr[1]/td[1]/a");
            var team1ScoreNode = game.SelectSingleNode(".//tr[1]/td[2]");
            var team2Node = game.SelectSingleNode(".//tr[2]/td[1]/a");
            var team2ScoreNode = game.SelectSingleNode(".//tr[2]/td[2]");

            if (team1Node == null || team1ScoreNode == null || team2Node == null || team2ScoreNode == null)
                continue;

            string team1 = team1Node.InnerText.Trim();
            string team2 = team2Node.InnerText.Trim();

            if (!int.TryParse(team1ScoreNode.InnerText.Trim(), out int team1Score) ||
                !int.TryParse(team2ScoreNode.InnerText.Trim(), out int team2Score))
                continue;

            if (team1Score == team2Score)
                continue;

            string winner = team1Score > team2Score ? team1 : team2;
            string loser = team1Score < team2Score ? team1 : team2;

            gameList.Add(new Game(team1, team1Score, team2, team2Score, winner, loser));
        }

        Email.Send(gameList);
    }
}