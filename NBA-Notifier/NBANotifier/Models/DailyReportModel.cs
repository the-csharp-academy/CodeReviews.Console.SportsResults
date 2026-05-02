namespace NBANotifier.Models;

public class DailyReportModel
{
    public string Date { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public required NbaResults NbaResults { get; set; }
    public required WnbaResults WnbaResults { get; set; }
    public required Subscriber Subscriber { get; set; }
}