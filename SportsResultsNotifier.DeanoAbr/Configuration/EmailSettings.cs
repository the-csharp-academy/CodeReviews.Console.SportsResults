namespace SportsResultsNotifier.DeanoAbr.Configuration;

public class EmailSettings
{
    public const string SectionName = "Email";
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
