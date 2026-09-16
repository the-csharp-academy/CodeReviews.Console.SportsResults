using SportsResultsNotifier.DeanoAbr;
using SportsResultsNotifier.DeanoAbr.Configuration;
using SportsResultsNotifier.DeanoAbr.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));
builder.Services.Configure<ScraperSettings>(builder.Configuration.GetSection(ScraperSettings.SectionName));

builder.Services.AddHttpClient("BasketballReference", client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("SportsResultsNotifier/1.0 (personal learning project; contact: your-email@example.com)");
});

builder.Services.AddSingleton<IScraperService, ScraperService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<FormatterService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
