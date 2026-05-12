namespace SportsResultsNotifier;

public class Game
{
    public string Team1 { get; }
    public int Team1Score { get; }
    public string Team2 { get; }
    public int Team2Score { get; }
    public string Winner { get; }
    public string Loser { get; }
    public int WinnerScore { get; }
    public int LoserScore { get; }

    public Game(string team1, int team1Score, string team2, int team2Score, string winner, string loser)
    {
        Team1 = team1;
        Team1Score = team1Score;
        Team2 = team2;
        Team2Score = team2Score;
        Winner = winner;
        Loser = loser;
        WinnerScore = winner == team1 ? team1Score : team2Score;
        LoserScore = loser == team1 ? team1Score : team2Score;
    }
}