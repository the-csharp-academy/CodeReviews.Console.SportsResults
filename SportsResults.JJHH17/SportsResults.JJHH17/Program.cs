
namespace SportsResults.JJHH17;

class Program
{
    static async Task Main(string[] args)
    {
        string content = SiteScrape.GetInfo();
        await Email.SendEmail(content);
    }
}
