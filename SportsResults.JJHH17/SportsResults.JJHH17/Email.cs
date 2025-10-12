using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;

namespace SportsResults.JJHH17
{
    public class Email
    {
        private static DateTime Now = DateTime.Now;
        private static string Date = Now.ToString("yyyy-MM-dd");
        private static string APIKey = ConfigurationManager.AppSettings["apiKey"];
        private static string senderAddress = ConfigurationManager.AppSettings["senderAddress"];
        private static string senderName = ConfigurationManager.AppSettings["senderName"];
        private static string recipientAddress = ConfigurationManager.AppSettings["recipientAddress"];
        private static string recipientName = ConfigurationManager.AppSettings["recipientName"];

        public static void SendEmail(string content)
        {

            var client = new SendGridClient(APIKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(senderAddress, senderName),
                Subject = $"Sports Results - {Date}",
                PlainTextContent = content,
            };

            msg.AddTo(new EmailAddress(recipientAddress, recipientName));
            var response = client.SendEmailAsync(msg);
            Console.WriteLine("Email sent");
        }
    }
}
