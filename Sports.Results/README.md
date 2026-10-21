# Sports Results Notifier

A C# console app that scrapes NBA game results from [Basketball Reference](https://www.basketball-reference.com) and emails them to a specified address.

## Requirements

- .NET 10 SDK
- A Gmail account with 2FA enabled (for SMTP)

## Setup

1. **Create a Gmail App Password**
   - Enable 2FA: [myaccount.google.com](https://myaccount.google.com) → Security → 2-Step Verification
   - Generate an app password: [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords)

2. **Configure secrets** (from the project root):

   ```bash
   dotnet user-secrets set "Mail:Password" "your-app-password"
   dotnet user-secrets set "Mail:Sender" "your-email@gmail.com"
   dotnet user-secrets set "Mail:Recipient" "your-email@gmail.com"
Run
dotnet run
Sends the previous day's NBA results to the configured recipient.

Daily Auto-Run (optional)
Use Windows Task Scheduler:

Open taskschd.msc
Create Task → Trigger: Daily, 08:00
Action: Start a program → C:\path\to\Sports.Results.exe
The app starts, scrapes, sends the email, and exits.