using System.Text;
using SportsResultsNotifier.DeanoAbr.Models;

namespace SportsResultsNotifier.DeanoAbr.Services;

public class FormatterService
{
    public string BuildBody(IReadOnlyList<GameResult> games, DateOnly date)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"NBA Game Results - {date:yyyy-MM-dd}");
        sb.AppendLine(new string('-', 40));

        if (games.Count == 0)
        {
            sb.AppendLine("No games played.");
            return sb.ToString();
        }

        foreach (var game in games)
        {
            sb.AppendLine($"{game.Winner} {game.WinnerScore} - {game.LoserScore} {game.Loser}");
        }

        return sb.ToString();
    }
}
