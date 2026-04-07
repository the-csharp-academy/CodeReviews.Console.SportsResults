namespace SportsResultsNotifier.TerrenceLGee.Data;

public static class ScrapingData
{
    public static string Url => @"https://www.basketball-reference.com/boxscores/";
    public static string XPathForGameDate => "//*[@id=\"content\"]/h1";
    public static string XPathForGameSummaries => "//div[contains(@class, 'game_summary')]";
    public static string XPathForWinnerName => ".//table[contains(@class, 'teams')]//tr[contains(@class, 'winner')]//a";
    public static string XPathForWinnerFinalScore => ".//table[contains(@class, 'teams')]//tr[contains(@class, 'winner')]//td[@class='right']";
    public static string XPathForLoserName => ".//table[contains(@class, 'teams')]//tr[contains(@class, 'loser')]//a";
    public static string XPathForLoserFinalScore => ".//table[contains(@class, 'teams')]//tr[contains(@class, 'loser')]//td[@class='right']";
    public static string XPathForScoresTable => ".//table[.//th[contains(text(), '1')]]";
    public static string XPathForRowsInScoresTable => ".//tbody/tr";
    public static string XPathForCellsInRow => ".//td";
    public static string XPathForTeamNameInScoresTable => ".//a";
}
