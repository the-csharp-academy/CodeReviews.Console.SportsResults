using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;

namespace SportsResults.JJHH17
{
    public class Email
    {
        private static string APIKey = ConfigurationManager.AppSettings["apiKey"];
        private static string senderAddress = ConfigurationManager.AppSettings["senderAddress"];
        private static string senderName = ConfigurationManager.AppSettings["senderName"];
        private static string recipientAddress = ConfigurationManager.AppSettings["recipientAddress"];
        private static string recipientName = ConfigurationManager.AppSettings["recipientName"];

        public static async Task SendEmail(string content)
        {
            var currentDate = DateTime.Now.ToString("dd-MM-yyyy");

            var client = new SendGridClient(APIKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(senderAddress, senderName),
                Subject = $"Sports Results - {currentDate}",
                PlainTextContent = content,
            };

            msg.AddTo(new EmailAddress(recipientAddress, recipientName));

            try
            {
                var response = await client.SendEmailAsync(msg);
                Console.WriteLine("Email sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred when sending the message: " + ex.Message);
            }
        }
    }
}
