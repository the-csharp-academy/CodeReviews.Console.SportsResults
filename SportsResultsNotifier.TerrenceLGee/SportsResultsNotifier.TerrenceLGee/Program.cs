using Serilog;
using SportsResultsNotifier.TerrenceLGee.Data;
using SportsResultsNotifier.TerrenceLGee.Interfaces;
using SportsResultsNotifier.TerrenceLGee.Services;

LoggingSetup();

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<EmailConfiguration>()
    .Bind(builder.Configuration.GetSection("EmailConfiguration"))
    .Validate(config =>
    !string.IsNullOrEmpty(config.SenderName) &&
    !string.IsNullOrEmpty(config.SenderEmail) &&
    !string.IsNullOrEmpty(config.ReceiverName) &&
    !string.IsNullOrEmpty(config.ReceiverEmail) &&
    !string.IsNullOrEmpty(config.Password) &&
    !string.IsNullOrEmpty(config.Host) &&
    config.Port > 0 && config.Port <= 65535, "Invalid Email Configuration")
    .ValidateOnStart();

builder.Services.AddScoped<IWebScraperService, WebScraperService>();
builder.Services.AddScoped<ISmtpClientFactory, SmtpClientFactory>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IFormatterService, FormatterService>();
builder.Services.AddSingleton<IRetryService, RetryService>();

builder.Services.AddHostedService<SportsResultsBackgroundService>();

var host = builder.Build();
host.Run();

void LoggingSetup()
{
    var loggingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    Directory.CreateDirectory(loggingDirectory);
    var filePath = Path.Combine(loggingDirectory, "log-.txt");
    var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File(
        path: filePath,
        rollingInterval: RollingInterval.Day,
        outputTemplate: outputTemplate)
        .CreateLogger();
}