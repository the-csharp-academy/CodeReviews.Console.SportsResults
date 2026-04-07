namespace SportsResultsNotifier.TerrenceLGee.Models;

public class Team
{
    public string? Name { get; set; }
    public string? FinalScore { get; set; }
    public List<string?> QuarterScores { get; set; } = [];
}
