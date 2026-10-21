using HtmlAgilityPack;

namespace Sports.Results
{
    public class HtmlGameParser
    {
        private readonly HtmlWeb _web;
        private readonly HtmlDocument _htmlDoc;
        private readonly HtmlNode _summary;

        public HtmlGameParser(string htmlPath)
        {
            _web = new HtmlWeb();
            _htmlDoc = _web.Load(htmlPath);
            _summary = _htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]/div[3]");
            if (_summary == null)
                throw new ArgumentException("Unable to load summary");
        }

        public List<Game> GetGames()
        {
            var games = new List<Game>();
            var date = GetDateTime();
            var divs = _summary.SelectNodes("./div");

            if (divs == null) 
                return games;

            foreach (var div in divs)
            {
                var loserRow = div.SelectSingleNode(".//tr[@class='loser']");
                var winnerRow = div.SelectSingleNode(".//tr[@class='winner']");
                if (loserRow == null || winnerRow == null) 
                    continue;

                var loser = CreateTeam(loserRow);
                var winner = CreateTeam(winnerRow);

                var status = GetStatus(loserRow) ?? GetStatus(winnerRow);
                games.Add(new Game(date, winner, loser, status));
            }
            return games;
        }

        private string? GetStatus(HtmlNode row)
        {
            var raw = row.SelectSingleNode("./td[3]")?.InnerText;
            if (raw == null) return null;
            var clean = raw
                .Replace("&nbsp;", "")
                .Replace("\u00A0", "")
                .Trim();
            return string.IsNullOrEmpty(clean) ? null : clean;
        }

        private Team CreateTeam(HtmlNode row)
        {
            return new Team(row.SelectSingleNode("./td[1]/a")?.InnerText, row.SelectSingleNode("./td[2]")?.InnerText);
        }

        public string? GetDateTime()
        {
            return _htmlDoc.DocumentNode.SelectNodes("//*[@class='button2 index']")?.First()?.InnerText;
        }
    }
}