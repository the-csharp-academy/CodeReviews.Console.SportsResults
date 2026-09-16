# Sports Results Notifier

Scrapes the previous day's NBA scores from Basketball Reference and emails them
once a day. Built on the .NET Generic Host (Worker Service), using HtmlAgilityPack
for scraping and MailKit for SMTP.

## Requirements traceability

| Requirement | Implementation |
|---|---|
| Read sports data from a website once a day | `Worker.cs` (24h loop), `Services/ScraperService.cs` |
| Send to a specific email address | `Services/EmailService.cs` + `EmailSettings` in `appsettings.json` |
| No interaction / runs automatically | `BackgroundService` hosted service (`Program.cs`) |
| Use the Agility Pack library for scraping | `HtmlAgilityPack` in `ScraperService.cs` |

## Configuration

Email settings are in `appsettings.json` under `Email`. Do **not** commit real
credentials — use `dotnet user-secrets` or environment variables:

```bash
dotnet user-secrets init
dotnet user-secrets set "Email:Password" "your-app-password"
```

## Run

```bash
dotnet run --project SportsResultsNotifier.DeanoAbr.csproj
```
