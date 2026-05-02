using NBANotifier.Interfaces;
using NBANotifier.Models;
using NBANotifier.Services;
using RazorLight;

namespace NBANotifier;

public class AppOrchestrator(
    IScrapingService scrapingService,
    IReportService reportService,
    ISubscriberService subscriberService,
    IEmailService emailService)
{
    public async Task RunAsync()
    {
        var engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Utils.GetTemplateDirectory())
            .UseMemoryCachingProvider()
            .Build();

        var nbaData = scrapingService.GetNbaResults();
        var wnbaData = scrapingService.GetWnbaResults();
        var subscribers = await subscriberService.GetSubscribersAsync();

        foreach (var subscriber in subscribers)
        {
            var model = new DailyReportModel
            {
                NbaResults = nbaData,
                WnbaResults = wnbaData,
                Subscriber = subscriber
            };

            var report = await reportService.BuildHtmlReportAsync(model);
            var email = emailService.BuildEmail(report, subscriber);
            await emailService.SendEmailAsync(email, subscriber);
        }
    }
}