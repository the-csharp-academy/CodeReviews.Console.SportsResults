namespace NBANotifier.Models;

public class Subscriber
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required bool SubscribeToWnbaReports { get; set; } = false;
}