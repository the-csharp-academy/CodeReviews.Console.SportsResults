using HtmlAgilityPack;
using SportsResultsNotifier.davetn657.Data;
using SportsResultsNotifier.davetn657.Services;
using System.Net.Mail;

namespace SportsResultsNotifier.davetn657;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailService"));
        builder.Services.AddSingleton<MailMessage>();
        builder.Services.AddSingleton<IEmailService, EmailService>();

        builder.Services.AddSingleton<HtmlWeb>();
        builder.Services.AddSingleton<IWebService, WebService>();

        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}