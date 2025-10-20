namespace MatchScoop.Console.Models;

public class Match
{
    public Match(Team home, Team away)
    {
        if (home is null || away is null)
        {
            throw new ArgumentException("Teams can't be null.");
        }

        Home = home;
        Away = away;
    }
    public Team Home { get; private set; }
    public Team Away { get; private set; }
}
