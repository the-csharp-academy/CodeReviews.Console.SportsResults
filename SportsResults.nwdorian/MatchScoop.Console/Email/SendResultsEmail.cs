using System.Globalization;
using System.Text;
using CSharpFunctionalExtensions;
using MailKit.Net.Smtp;
using MatchScoop.Console.Models;
using MatchScoop.Console.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MatchScoop.Console.Email;

public class SendResultsEmail
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<SendResultsEmail> _logger;

    public SendResultsEmail(IOptions<EmailOptions> emailOptions, ILogger<SendResultsEmail> logger)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task<Result> Handle(IEnumerable<Match> matches)
    {
        try
        {
            var message = new MimeMessage();
            var sender = new MailboxAddress("Match Scoop", _emailOptions.UserEmail);
            message.From.Add(sender);
            var recipient = new MailboxAddress(_emailOptions.RecipientName, _emailOptions.RecipientEmail);
            message.To.Add(recipient);
            message.Subject = $"Match Scoop NBA Results for {DateTime.Now:d}";

            var bb = new BodyBuilder
            {
                HtmlBody = CreateMessageBody(matches)
            };
            message.Body = bb.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailOptions.UserEmail, _emailOptions.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully!");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error!");
            return Result.Failure("There was an error: " + ex.Message);
        }
    }

    private static string CreateMessageBody(IEnumerable<Match> matches)
    {
        var matchList = matches.ToList();

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(CultureInfo.InvariantCulture,
            $"""
            <html>
                <body>
                    <h1>NBA games report</h1>
                    <br/>
            """);

        if (matchList.Count == 0)
        {
            stringBuilder.Append("""
                        <p>No games were played today!<p/>
                    </body>
                </html>
                """);
            return stringBuilder.ToString();
        }

        stringBuilder.Append("""
            <table>
                <thead>
                    <tr>
                        <th>Home</th>
                        <th>Score</th>
                        <th>-</th>
                        <th>Score</th>
                        <th>Away</th>
                    </tr>
                </thead>
                <tbody>
            """);

        foreach (var match in matchList)
        {
            stringBuilder.Append(CultureInfo.InvariantCulture,
                $"""
                    <tr>
                        <td style="text-align:center">{match.Home.Name}</td>
                        <td style="text-align:right">{match.Home.Score}</td>
                        <td>-</td>
                        <td>{match.Away.Score}</td>
                        <td style="text-align:center">{match.Away.Name}</td>
                    </tr>
                    <tr></tr>
                """);
        }

        stringBuilder.Append("""
                        </tbody>
                    </table>
                </body>
            </html>
        """);

        return stringBuilder.ToString();
    }
}
