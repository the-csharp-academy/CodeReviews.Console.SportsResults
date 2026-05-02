using NBANotifier.Models;

namespace NBANotifier.Interfaces;

public interface ISubscriberRepo
{
    public Task<List<Subscriber>> GetSubscribersAsync();
}