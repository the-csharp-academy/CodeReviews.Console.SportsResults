using SportsResultsNotifier.TerrenceLGee.Data;

namespace SportsResultsNotifier.TerrenceLGee.Tests.Resources;

public static class EmailResources
{
    public static EmailData GetEmailData()
    {
        return new()
        {
            Subject = "This is to test email functionality",
            Body = "Unit testing the email service"
        };
    }

    public static EmailConfiguration GetConfiguration()
    {
        return new()
        {
            SenderEmail = "h_simpson@example.com",
            SenderName = "Homer J. Simpson",
            ReceiverEmail = "mSimpson@example.com",
            ReceiverName = "Marge Simpson",
            Host = "smtp.test.com",
            Password = "Te$tP@$$w0rd",
            Port = 123,
        };
    }
}
