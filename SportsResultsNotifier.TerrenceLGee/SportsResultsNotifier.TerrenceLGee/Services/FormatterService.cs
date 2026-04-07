using SportsResultsNotifier.TerrenceLGee.Interfaces;
using SportsResultsNotifier.TerrenceLGee.Models;
using System.Net;
using System.Text;

namespace SportsResultsNotifier.TerrenceLGee.Services;

public class FormatterService : IFormatterService
{
    public string FormatGameResults(List<GameSummary> games)
    {
        if (games is null || games.Count == 0)
        {
            return "<html><body><h1>No Games Played Today</h1></body></html>";
        }

        var formattedText = new StringBuilder();

        BuildHeader(formattedText, games[0].GameDate);

        var gameNumber = 1;

        foreach (var game in games)
        {
            formattedText.AppendLine($"<div style='margin-bottom:20px;'><h5>Game {gameNumber++}</h5></div>");
            formattedText.AppendLine("<div>");

            formattedText.AppendLine("<table style='border-collapse:collapse;'>");
            formattedText.AppendLine("<tbody>");

            BuildTeamTable(formattedText, game.Winner);
            BuildTeamTable(formattedText, game.Loser);

            formattedText.AppendLine("</tbody>");
            formattedText.AppendLine("</table>");

            formattedText.AppendLine("<table style='border-collapse:collapse;'>");
            formattedText.AppendLine("<thead>");

            BuildScoresTableHeader(formattedText, game.Winner.QuarterScores.Count);

            formattedText.AppendLine("</thead>");

            formattedText.AppendLine("<tbody>");

            BuildScoresTable(formattedText, game.Winner);
            BuildScoresTable(formattedText, game.Loser);

            formattedText.AppendLine("</tbody>");
            formattedText.AppendLine("</table>");

            formattedText.AppendLine("</div>");
        }

        BuildFooter(formattedText);

        return formattedText.ToString();
    }

    private void BuildHeader(StringBuilder formattedText, string? gameDate)
    {
        formattedText.AppendLine("<html>");
        formattedText.AppendLine("<body>");
        formattedText.AppendLine("<div class='header'>");
        formattedText.AppendLine($"<h2>{gameDate ?? "Game Date N/A"}</h2>");
        formattedText.AppendLine("</div>");
    }

    private void BuildFooter(StringBuilder formattedText)
    {
        formattedText.Append("</body>");
        formattedText.Append("</html>");
    }

    private void BuildTeamTable(StringBuilder formattedText, Team team)
    {
        formattedText.AppendLine("<tr>");
        formattedText.AppendLine($"<td style='border:1px solid #ccc'>{WebUtility.HtmlEncode(team.Name)}</td>");
        formattedText.AppendLine($"<td style='border:1px solid #ccc'>{team.FinalScore}</td>");
        formattedText.AppendLine("</tr>");
    }

    private void BuildScoresTableHeader(StringBuilder formattedText, int scoresCount)
    {
        formattedText.AppendLine("<tr>");
        formattedText.AppendLine("<th>Quarter Scores</th>");

        for (var score = 1; score <= scoresCount; score++)
        {
            formattedText.AppendLine($"<th>{score}</th>");
        }

        formattedText.Append("</tr>");
    }

    private void BuildScoresTable(StringBuilder formattedText, Team team)
    {
        formattedText.AppendLine("<tr>");
        formattedText.AppendLine($"<td style='border:1px solid #ccc'>{WebUtility.HtmlEncode(team.Name)}</td>");

        foreach (var score in team.QuarterScores)
        {
            formattedText.AppendLine($"<td style='border:1px solid #ccc'>{score}</td>");
        }

        formattedText.AppendLine("</tr>");
    }
}
