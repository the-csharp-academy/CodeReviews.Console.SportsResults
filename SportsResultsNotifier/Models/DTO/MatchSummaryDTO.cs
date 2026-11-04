namespace SportsResultsNotifier.Models.DTO;

internal class MatchSummaryDTO
{
    public string? WinnerName { get; set; }
    public string? WinnerTotalPoints { get; set; }
    public string[]? WinnerQuarterPoints { get; set; }
    
    public string? LoserName { get; set; }
    public string? LoserTotalPoints { get; set; }
    public string[]? LoserQuarterPoints { get; set; }

    public string? PtsPlayerName { get; set; }
    public string? PtsPoints { get; set; }

    public string? TrbPlayerName { get; set; }
    public string? TrbPoints { get; set; }
}

