using NBANotifier.Models;

namespace NBANotifier.Data;

public class DbSeeder
{
    public static async Task SeedTestSubscribersAsync(AppDbContext db)
    {
        var subscribers = new List<Subscriber>
        {
            new() { Name = "John Doe", Email = "FakeEmail@gmail.com", SubscribeToWnbaReports = false },
            new() { Name = "Jane Doe", Email = "TestEmail@hotmail.com", SubscribeToWnbaReports = true },
            new() { Name = "Jackson Doe", Email = "DevelopmentEmail@outlook.com", SubscribeToWnbaReports = true }
        };

        await db.Subscribers.AddRangeAsync(subscribers);
        await db.SaveChangesAsync();
    }
}