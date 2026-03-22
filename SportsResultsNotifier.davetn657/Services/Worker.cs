namespace SportsResultsNotifier.davetn657.Services;

public class Worker : BackgroundService
{
    private readonly IWebService _webService;
    private readonly IEmailService _emailSerivce;
    private readonly ILogger _logger;

    public Worker(ILogger<Worker> logger, IWebService webService, IEmailService emailService)
    {
        _logger = logger;
        _webService = webService;
        _emailSerivce = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var subject = _webService.GetTitle();
                var games = _webService.GetAllGameData();

                if(subject == string.Empty || games.Count == 0)
                {
                    Console.WriteLine("Could not recieve basketball data!");
                    Console.WriteLine("Web service may be down...");
                }
                else
                {
                    _emailSerivce.SendEmail(subject, games);
                }
            }

            var today = DateTime.Now;
            var timeDelay = new DateTime(today.Year, today.Month, today.Day + 1, 07, 00, 00);

            await Task.Delay(timeDelay - today, stoppingToken);
        }
    }
}