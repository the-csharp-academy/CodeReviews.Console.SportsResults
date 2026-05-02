using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NBANotifier;
using NBANotifier.Data;
using NBANotifier.Interfaces;
using NBANotifier.Models;
using NBANotifier.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "Configuration"))
    .AddJsonFile("appSettings.json", false, true);

var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>() ??
                    throw new NullReferenceException("Email settings not found");
builder.Services.AddSingleton(emailSettings);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(Utils.GetDatabaseConnectionString()));

builder.Services.AddTransient<ISubscriberRepo, SubscriberRepo>();

builder.Services.AddTransient<IScrapingService, ScrapingService>();
builder.Services.AddTransient<IReportService, ReportService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ISubscriberService, SubscriberService>();

builder.Services.AddSingleton<AppOrchestrator>();

var app = builder.Build();


if (args.Contains("--refresh-snapshots"))
{
    var scraper = app.Services.GetRequiredService<IScrapingService>();
    scraper.UpdateSnapshots();
    return;
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var dbDirectory = Utils.GetDatabaseDirectory();
    Directory.CreateDirectory(dbDirectory);

    var dbPath = Path.Combine(dbDirectory, "app.db");
    var isFirstRun = !File.Exists(dbPath);

    await db.Database.MigrateAsync();

    if (isFirstRun) await DbSeeder.SeedTestSubscribersAsync(db);
}

var orchestrator = app.Services.GetRequiredService<AppOrchestrator>();
await orchestrator.RunAsync();