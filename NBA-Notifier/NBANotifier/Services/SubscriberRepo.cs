using Microsoft.EntityFrameworkCore;
using NBANotifier.Data;
using NBANotifier.Interfaces;
using NBANotifier.Models;

namespace NBANotifier.Services;

public class SubscriberRepo(AppDbContext db) : ISubscriberRepo
{
    public async Task<List<Subscriber>> GetSubscribersAsync()
    {
        return await db.Subscribers.ToListAsync();
    }
}