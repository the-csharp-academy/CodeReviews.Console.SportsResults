using Fluid;
using SportsResultsNotifier.Models;
using SportsResultsNotifier.Models.DTO;
using SportsResultsNotifier.Models.Options;
using SportsResultsNotifier.Services;

namespace SportsResultsNotifier
{
    internal class Program
    {
        static void Main()
        {
            // Scrapping

            var basketballScraper = new BasketballGamesScraper()
            {
                DataDate = new DateOnly(2026, 11, 2)
            };

            var matchesData = basketballScraper.CollectData();

            if (matchesData == null)
            {
                Console.WriteLine("No games played.");
                return;
            }


            // Embedding in HTML

            string relativePath = Path.Combine("Templates", "Emails", "MatchSummaries.liquid");
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            string? HtmlMessage = null;

            var options = new TemplateOptions();
            options.MemberAccessStrategy.Register<MatchSummaryDTO>();

            var parser = new FluidParser();
            if (parser.TryParse(File.ReadAllText(fullPath), out var template, out var error))
            {
                
                var context = new TemplateContext(options);
                context.SetValue("matches", matchesData);

           
                HtmlMessage =  template.Render(context);
            }

            if (HtmlMessage == null)
                Console.WriteLine("Failed to parse the template to HTML!");
            

            // Sending

            EmailSender emailSender = new()
            {
                Options = new EmailOptions
                {
                    SenderEmail = "sender@gmail.com",
                    Password = "password",
                    //EnableSSL = true,
                    //Host = "smtp.gmail.com",
                    //Port = 587
                }
            };

            var message = new EmailMessage
            {
                RecipientEmail = "reciever@gmail.com",
                Subject = "NBA Games Played 🏀",
                Body = HtmlMessage!,
                IsBodyHtml = true
            };

            emailSender.SendEmail(message);

            Console.WriteLine("Email sent.");
        }
    }
}
