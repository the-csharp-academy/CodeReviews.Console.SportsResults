namespace NBANotifier.Models;

public class NbaResults
{
    public List<Game> Games { get; set; }
    public required List<Team> WesternConference { get; set; }
    public required List<Team> EasternConference { get; set; }
}