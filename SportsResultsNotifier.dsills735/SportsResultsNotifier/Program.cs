namespace SportsResultsNotifier;

public class Program
{
    public static void Main(string[] args)
    {
        TimeOnly runningTime = new TimeOnly(23, 0, 0); // 11 PM
        DateOnly lastRun = DateOnly.MinValue;

        while (true)
        {
            TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            if (now >= runningTime && lastRun < today)
            {
                BasketballScanner.Scan();
                lastRun = today;
            }

            Thread.Sleep(10000);
        }
    }
}