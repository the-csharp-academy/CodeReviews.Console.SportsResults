using HtmlAgilityPack;
using System.Net.Mail;
using System.Text;

string body = Scrape();

SendEmail("Daily Quotes", body);
static string Scrape()
{
    var web = new HtmlWeb();
    HtmlDocument doc = web.Load("https://quotes.toscrape.com/"); //Load the website
    var quotes = doc.DocumentNode.SelectNodes("//div[@class='quote']");
    StringBuilder stringBuilder = new StringBuilder();

    if (quotes == null)
    {
        Console.WriteLine("No quotes found");
        return "";
    }
    Console.WriteLine($"Quotes found: {quotes.Count}");

    foreach (var quote in quotes)
    {
        string text = HtmlEntity.DeEntitize(quote.SelectSingleNode(".//span[@class='text']").InnerText.Trim());
        string author = quote.SelectSingleNode(".//small[@class='author']").InnerText.Trim();
        var tagNodes = quote.SelectNodes(".//a[@class='tag']");
        var tagList = new List<string>();

        foreach (var tag in tagNodes)
        {
            tagList.Add(tag.InnerText.Trim());
        }

        string tags = string.Join(", ", tagList);


        foreach (var tag in tagNodes)
        {
            Console.WriteLine(tag.InnerText.Trim());
        }

        if (tagNodes == null)
        {
            Console.WriteLine("No tags found");
            continue;
        }

        stringBuilder.AppendLine($"{text} - {author} [{tags}]\n");
        Console.WriteLine($"{text} - {author} [{tags}]");
    }
    return stringBuilder.ToString();
}

static void SendEmail(string subject, string body)
{
    using var mail = new MailMessage();
    using var smtp = new SmtpClient("localhost", 25);

    mail.From = new MailAddress("sender@example.com");
    mail.To.Add("recipient@example.com");
    mail.Subject = subject;
    mail.Body = body;

    smtp.Send(mail);
}