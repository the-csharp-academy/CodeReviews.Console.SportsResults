namespace SportsResultsNotifier.Models;

internal class EmailMessage
{
    public required string RecipientEmail { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public bool IsBodyHtml { get; set; } = false;
}

