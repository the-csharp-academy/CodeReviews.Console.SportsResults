using Microsoft.Extensions.Configuration;

namespace Sports.Results
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var dateForReport = DateTime.Now.AddDays(-1);
            var url = $"https://www.basketball-reference.com/boxscores/?month={dateForReport.Month}&day={dateForReport.Day}&year={dateForReport.Year}";

            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var sender = config["Mail:Sender"];
            var password = config["Mail:Password"];
            var recipient = config["Mail:Recipient"];

            var parser = new HtmlGameParser(url);
            var games = parser.GetGames();

            var message = string.Join("\n", games.Select(g => g.ToString()));
            var mailSender = new MailSender(sender, password);
            await mailSender.SandMail(recipient, $"NBA Results {parser.GetDateTime()}", message);
        }
    }
}