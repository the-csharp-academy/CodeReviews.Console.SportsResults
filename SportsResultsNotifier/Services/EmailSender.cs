using SportsResultsNotifier.Models;
using SportsResultsNotifier.Models.Options;
using System.Net;
using System.Net.Mail;

namespace SportsResultsNotifier.Services;

internal class EmailSender
{
    public required EmailOptions Options { private get; set; }

    public void SendEmail(EmailMessage messageInfo)
    {
        using (MailMessage mail = new())
        {
            mail.From = new MailAddress(Options.SenderEmail);
            mail.To.Add(messageInfo.RecipientEmail);
            mail.Subject = messageInfo.Subject;
            mail.Body = messageInfo.Body;
            mail.IsBodyHtml = messageInfo.IsBodyHtml;
            //mail.Attachments.Add(new Attachment("D:\\TestFile.txt")); // attachments
            
            using (SmtpClient smtp = new(Options.Host, Options.Port)) // MailKit is recommended
            {
                smtp.Credentials = new NetworkCredential(Options.SenderEmail, Options.Password);
                smtp.EnableSsl = Options.EnableSSL;
                try
                {
                    smtp.Send(mail);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Error: {ex.InnerException}");
                    Console.WriteLine($"Error: {ex.StackTrace}");
                }
            }
        }
    }
}


