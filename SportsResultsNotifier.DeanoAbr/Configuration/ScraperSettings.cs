namespace SportsResultsNotifier.DeanoAbr.Configuration;

public class ScraperSettings
{
    public const string SectionName = "Scraper";
    public string Url { get; set; } = "https://www.basketball-reference.com/boxscores/";
}
