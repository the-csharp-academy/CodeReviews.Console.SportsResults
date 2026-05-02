using NBANotifier.Interfaces;
using NBANotifier.Models;

namespace NBANotifier.Services;

public class SubscriberService(ISubscriberRepo repo) : ISubscriberService
{
    public async Task<List<Subscriber>> GetSubscribersAsync()
    {
        return await repo.GetSubscribersAsync();
    }
}