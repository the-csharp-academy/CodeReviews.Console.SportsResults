namespace SportsResultsNotifier.davetn657.Data;

public class EmailSettings
{
    public string SmtpAddress { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Password { get; set; } = string.Empty;
}