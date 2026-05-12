using System.Net.Mail;

namespace SportsResultsNotifier;

internal class Email
{
    public static void Send(List<Game> games)
    {
        string smtpAddress = "smtp.gmail.com";
        int portNumber = 587;
        bool enableSSL = true;
        string fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM")
            ?? throw new InvalidOperationException("EMAIL_FROM environment variable is not set.");
        string toEmail = Environment.GetEnvironmentVariable("EMAIL_TO")
            ?? throw new InvalidOperationException("EMAIL_TO environment variable is not set.");
        string password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
            ?? throw new InvalidOperationException("EMAIL_PASSWORD environment variable is not set.");

        string subject = "Sports Results Notification";
        string body;

        if (games != null && games.Count > 0)
        {
            body = "Here are the latest sports results:\n";
            foreach (Game game in games)
            {
                body += $"\n\n{game.Winner} beat the {game.Loser} with a score of {game.WinnerScore} to {game.LoserScore}";
            }
        }
        else
        {
            body = "No NBA games were found.";
        }

        using MailMessage mail = new MailMessage();
        mail.From = new MailAddress(fromEmail);
        mail.To.Add(toEmail);
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = false;

        using SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);
        smtp.Credentials = new System.Net.NetworkCredential(fromEmail, password);
        smtp.EnableSsl = enableSSL;
        smtp.Send(mail);
    }

}