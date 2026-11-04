namespace SportsResultsNotifier.Models.Options;

internal class EmailOptions
{
    public required string SenderEmail { get; set; }
    public required string Password { get; set; }
    public string Host { get; set; } = "127.0.0.1"; // localhost for development
    public int Port { get; set; } = 25; // Papercut-SMTP
    public bool EnableSSL { get; set; } = false; // Papercut-SMTP doesn't support encryption
}

