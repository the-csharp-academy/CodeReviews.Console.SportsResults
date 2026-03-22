using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SportsResultsNotifier.davetn657.Data;
using System.Net.Mail;

namespace SportsResultsNotifier.davetn657.Tests;

public class EmailService
{
    [Fact]
    public void SendEmail_Success()
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var settings = builder.Build().GetSection("EmailService");

        var emailSettings = Options.Create(new EmailSettings
        {
            From = settings["From"],
            To = settings["To"],
            Port = Int32.Parse(settings["Port"]),
            SmtpAddress = settings["SmtpAddress"],
            Password = settings["Password"]
        });

        var emailService = new Services.EmailService(new MailMessage(), emailSettings);

        var expected = "Successfully sent email";

        var actual = emailService.SendEmail("Send Email Success Test", new List<GameData>());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SendEmail_Failiure()
    {
        var emailSettings = Options.Create(new EmailSettings
        {
            From = "test@email.com",
            To = "test@email.com",
            Port = 9999,
            SmtpAddress = "localhost",
            Password = "pass"
        });

        var emailService = new Services.EmailService(new MailMessage(), emailSettings);

        var expected = string.Empty;

        var actual = emailService.SendEmail("Send Email Success Test", new List<GameData>());

        Assert.Equal(expected, actual);
    }
}