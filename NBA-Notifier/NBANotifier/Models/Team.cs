namespace NBANotifier.Models;

public class Team
{
    public required string Name { get; set; }
    public required string Wins { get; set; }
    public required string Losses { get; set; }
    public required string WinLossPercentage { get; set; }
    public required string GamesBack { get; set; }
    public required string PointsPerGame { get; set; }
    public required string PointsAllowedPerGame { get; set; }
}