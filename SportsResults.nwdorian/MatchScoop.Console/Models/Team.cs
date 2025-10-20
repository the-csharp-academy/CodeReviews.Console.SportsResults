namespace MatchScoop.Console.Models;

public class Team
{
    public Team(string name, int score)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Team name is required.");
        }

        Name = name;
        Score = score;
    }
    public string Name { get; private set; }
    public int Score { get; private set; }
}
