using HtmlAgilityPack;

namespace SportsResults.JJHH17
{
    public class SiteScrape
    {
        public static string GetInfo()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://www.basketball-reference.com/boxscores/");

            var title = document.DocumentNode
                .SelectSingleNode("//h1").InnerText;
            var team1Name = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[1]/tbody/tr[1]/td[1]").InnerText;
            var team1Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[1]/tbody/tr[1]/td[2]").InnerText;
            var team2Name = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[2]/tbody/tr[2]/td[1]/a").InnerText;
            var team2Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[1]/tbody/tr[2]/td[2]").InnerText;

            var team1Q1Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[2]/tbody/tr[1]/td[2]").InnerText;
            var team1Q2Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[2]/tbody/tr[1]/td[3]").InnerText;
            var team1Q3Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[2]/tbody/tr[1]/td[4]").InnerText;
            var team1Q4Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[2]/tbody/tr[1]/td[5]").InnerText;

            var team2Q1Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[2]/tbody/tr[2]/td[2]").InnerText;
            var team2Q2Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[2]/tbody/tr[2]/td[3]").InnerText;
            var team2Q3Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[2]/tbody/tr[2]/td[4]").InnerText;
            var team2Q4Score = document.DocumentNode
                .SelectSingleNode("//*[@id=\"content\"]/div[3]/div/table[2]/tbody/tr[2]/td[5]").InnerText;

            string fullContent =
                $"{title}\n\n{team1Name}: {team1Score}\n{team2Name}: {team2Score}\n\nQuarter Scores\n\n{team1Name}" +
                $"\nQ1: {team1Q1Score}\nQ2: {team1Q2Score}\nQ3: {team1Q3Score}\nQ4: {team1Q4Score}\n\n{team2Name}" +
                $"\nQ1: {team2Q1Score}\nQ2: {team2Q2Score}\nQ3: {team2Q3Score}\nQ4: {team2Q4Score}";

            return fullContent;
        }
    }
}
