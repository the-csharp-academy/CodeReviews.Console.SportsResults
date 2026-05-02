using NBANotifier.Models;

namespace NBANotifier.Interfaces;

public interface IScrapingService
{
    public void UpdateSnapshots();
    public NbaResults GetNbaResults();
    public WnbaResults GetWnbaResults();
}