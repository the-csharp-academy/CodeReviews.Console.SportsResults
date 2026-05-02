namespace NBANotifier.Models;

public class Game
{
    public required string HomeTeam { get; set; }
    public required string AwayTeam { get; set; }
    public required int HomeScore { get; set; }
    public required int AwayScore { get; set; }
}