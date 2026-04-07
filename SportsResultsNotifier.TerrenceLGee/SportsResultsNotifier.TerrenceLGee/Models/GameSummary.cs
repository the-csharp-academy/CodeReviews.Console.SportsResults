namespace SportsResultsNotifier.TerrenceLGee.Models;

public class GameSummary
{
    public string? GameDate { get; set; }
    public Team Winner { get; set; } = default!;
    public Team Loser { get; set; } = default!;
}
