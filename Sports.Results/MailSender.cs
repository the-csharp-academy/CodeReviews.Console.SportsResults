using MimeKit;
using MailKit.Net.Smtp;
namespace Sports.Results
{
    public class MailSender
    {
        private readonly string _sender;
        private readonly string _password;
        public MailSender(string sender, string password)
        {
            _sender = sender;
            _password = password;
        }
        public async Task SandMail(string email, string subject, string message)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("SportresultParser", _sender));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(_sender, _password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }

}