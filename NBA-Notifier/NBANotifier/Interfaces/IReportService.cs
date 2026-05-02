using NBANotifier.Models;

namespace NBANotifier.Interfaces;

public interface IReportService
{
    public Task<string> BuildHtmlReportAsync(DailyReportModel model);
}