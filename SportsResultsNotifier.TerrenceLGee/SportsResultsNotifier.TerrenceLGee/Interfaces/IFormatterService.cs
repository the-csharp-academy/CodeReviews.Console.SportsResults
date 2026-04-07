using SportsResultsNotifier.TerrenceLGee.Models;

namespace SportsResultsNotifier.TerrenceLGee.Interfaces;

public interface IFormatterService
{
    string FormatGameResults(List<GameSummary> games);
}
