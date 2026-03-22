namespace SportsResultsNotifier.davetn657.Data;

public class GameData
{
    public string Winner { get; set; } = string.Empty;
    public string Loser { get; set; } = string.Empty;
    public string WinnerScore { get; set; } = string.Empty;
    public string LoserScore { get; set; } = string.Empty;
    public List<string> WinnerRounds { get; set; } = [];
    public List<string> LoserRounds { get; set; } = [];
}