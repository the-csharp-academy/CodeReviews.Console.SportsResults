namespace MatchScoop.Console.Options;

public class EmailOptions
{
    public string? UserEmail { get; set; }
    public string? Password { get; set; }
    public string? SmtpServer { get; set; }
    public int Port { get; set; }
    public string? RecipientEmail { get; set; }
    public string? RecipientName { get; set; }

}
