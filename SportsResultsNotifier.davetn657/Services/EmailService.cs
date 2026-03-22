using Microsoft.Extensions.Options;
using SportsResultsNotifier.davetn657.Data;
using System.Net;
using System.Net.Mail;

namespace SportsResultsNotifier.davetn657.Services;

public interface IEmailService
{
    /*
     * Send Email 
     */
    public string SendEmail(string subject, List<GameData> gameData);
}

public class EmailService : IEmailService
{
    private readonly MailMessage _mail;
    private readonly EmailSettings _settings;

    public EmailService(MailMessage mail, IOptions<EmailSettings> settings)
    {
        _mail = mail;
        _settings = settings.Value;

        _mail.From = new MailAddress(_settings.From);
        _mail.To.Add(_settings.To);
    }

    public string SendEmail(string subject, List<GameData> games)
    {
        _mail.Subject = subject;
        _mail.Body = FormatDataToHtml(games);
        _mail.IsBodyHtml = true;

        try
        {
            using (var smtp = new SmtpClient(_settings.SmtpAddress, _settings.Port))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_settings.From, _settings.Password);
                smtp.EnableSsl = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(_mail);
            }

            return "Successfully sent email";
        }
        catch
        {
            return string.Empty;
        }

        
    }

    public string FormatDataToHtml(List<GameData> games)
    {
        var html = "<html>";
        var css = @"<style>
                    h1{  
                        text-align: center;
                    }
                    table.center{
                        margin-left: auto;
                        margin-right: auto;
                        width: 60%;
                        border-collapse: collapse;
                        border: 1px solid black;
                    }
                    th, td{
                    text-align: center;
                    padding: 8px;
                    }
                    </style>";

        html += css;

        foreach(var game in games)
        {
            html += @$"
                        <div>
	                        <h1>{game.WinnerScore} - {game.LoserScore}</h1>
	                        <h1>{game.Winner} v.s {game.Loser}</h1>
	                        <table class=""center"">
		                        <tr>
        	                        <th>Team</th>
			                        <th>Round 1</th>
    		                        <th>Round 2</th>
    		                        <th>Round 3</th>
    		                        <th>Round 4</th>
		                        </tr>
                                <tr>
        	                        <td>{game.Winner}</td>
        	                        <td>{game.WinnerRounds[0]}</td>
                                    <td>{game.WinnerRounds[1]}</td>
                                    <td>{game.WinnerRounds[2]}</td>
                                    <td>{game.WinnerRounds[3]}</td>
                                </tr>
                                <tr>
        	                        <td>{game.Loser}</td>
        	                        <td>{game.LoserRounds[0]}</td>
                                    <td>{game.LoserRounds[1]}</td>
                                    <td>{game.LoserRounds[2]}</td>
                                    <td>{game.LoserRounds[3]}</td>
                                </tr>
	                        </table>
                        </div>
                      ";
        }

        html += "</html>";
        return html;
    }
}