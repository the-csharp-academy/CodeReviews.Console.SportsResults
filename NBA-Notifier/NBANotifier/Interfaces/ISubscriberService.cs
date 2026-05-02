using NBANotifier.Models;

namespace NBANotifier.Interfaces;

public interface ISubscriberService
{
    public Task<List<Subscriber>> GetSubscribersAsync();
}