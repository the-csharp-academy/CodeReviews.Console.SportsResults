namespace NBANotifier.Models;

public class EmailSettings
{
    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string SmtpUser { get; set; }
    public required string SmtpPassword { get; set; }
    public required string SmtpHost { get; set; }
    public required int SmtpPort { get; set; }
    public required bool UseTls { get; set; }
}