# Sports Results Notifier — Implementation Plan

## Goal

Build a .NET 10 Worker Service that scrapes the previous day's NBA scores from
Basketball Reference (via HtmlAgilityPack) once a day and emails them via
MailKit, submitted as a C# Academy code-review PR.

## Current context / assumptions

- **Repo** is a fork: `origin` = `DeanoAbr/CodeReviews.Console.SportsResults`,
  `upstream` = `the-csharp-academy/CodeReviews.Console.SportsResults`. `main` is
  currently at upstream's "clean up" state — contains only a shared `.gitignore`
  (CRLF, Visual Studio gitignore), no code.
- **Do not modify the root `.gitignore`.** It already ignores `bin/`, `obj/`,
  `Debug/`, `Release/`, etc., so the submission folder needs no extra ignore rules.
- **Convention** (verified from 3 real merged PRs): each submission is ONE
  top-level folder named `<Project>.<GitHubHandle>`, containing the solution +
  projects. Handle is `DeanoAbr` (from the `origin` remote URL), so the folder is
  `SportsResultsNotifier.DeanoAbr`. The PR diff must be *only* additions under
  that folder.
- **Toolchain**: only .NET 10 SDK (10.0.401) + .NET 10 runtime are installed (no
  .NET 8 runtime) → target `net10.0`. The academy's own recent submissions also
  target `net10.0`.
- **Sending real email** needs SMTP credentials the implementer cannot invent.
  The plan builds and unit-tests everything except the live email send, which is
  smoke-tested against a local throwaway SMTP catcher (Papercut) and left for the
  user to configure with real creds.
- **Git**: local `git add`/`git commit` work; `git push` and the PR are the
  **user's** manual steps (this host cannot push to GitHub). The implementer
  builds on a dedicated `submission` branch.

## Architecture / approach

A `Microsoft.NET.Sdk.Worker` project (BackgroundService) hosted in the .NET
Generic Host. `Worker` loops every 24h: `ScraperService` fetches and parses the
box-scores page into `GameResult` records, `FormatterService` renders a plain-text
email body, `EmailService` sends it via MailKit. Parsing is split out of HTTP
fetching so it is unit-testable against a canned HTML string; email config lives
in `appsettings.json` bound to an options class. Scraping and formatting are
covered by xUnit tests (TDD); the HTTP fetch and SMTP send are thin, verified by a
manual smoke run.

---

## Step-by-step tasks

### Phase 0 — Branch + scaffold

**Task 0.1 — Create the submission branch (do not restructure `main`).**

```bash
cd "C:/Users/deant/OneDrive/Dokumentumok/GitHub/CodeReviews.Console.SportsResults"
git checkout -b submission main
```

Verify: `git branch --show-current` → `submission`.

**Task 0.2 — Scaffold the two projects + solution.**

```bash
mkdir -p SportsResultsNotifier.DeanoAbr
cd SportsResultsNotifier.DeanoAbr
dotnet new worker -n SportsResultsNotifier.DeanoAbr -o . --framework net10.0
dotnet new xunit -n SportsResultsNotifier.DeanoAbr.Tests -o ../SportsResultsNotifier.DeanoAbr.Tests --framework net10.0
dotnet new sln -n SportsResultsNotifier.DeanoAbr --format slnx
dotnet sln add SportsResultsNotifier.DeanoAbr.csproj ../SportsResultsNotifier.DeanoAbr.Tests/SportsResultsNotifier.DeanoAbr.Tests.csproj
cd ..
```

Verify: `dotnet build` from repo root succeeds with `Build succeeded` and the
tree contains:
```
SportsResultsNotifier.DeanoAbr/SportsResultsNotifier.DeanoAbr.csproj
SportsResultsNotifier.DeanoAbr/SportsResultsNotifier.DeanoAbr.slnx
SportsResultsNotifier.DeanoAbr.Tests/SportsResultsNotifier.DeanoAbr.Tests.csproj
```

**Task 0.3 — Add NuGet packages.**

```bash
cd SportsResultsNotifier.DeanoAbr
dotnet add package HtmlAgilityPack
dotnet add package MailKit
dotnet add package Microsoft.Extensions.Http
cd ../SportsResultsNotifier.DeanoAbr.Tests
dotnet add reference ../SportsResultsNotifier.DeanoAbr/SportsResultsNotifier.DeanoAbr.csproj
rm -f UnitTest1.cs
cd ..
```

Verify: `dotnet restore` succeeds; `dotnet build` still succeeds.

---

### Phase 1 — Model + config (no tests needed for records)

**Task 1.1 — `SportsResultsNotifier.DeanoAbr/Models/GameResult.cs`.**

```csharp
namespace SportsResultsNotifier.DeanoAbr.Models;

public record GameResult(string Winner, int WinnerScore, string Loser, int LoserScore);
```

**Task 1.2 — `SportsResultsNotifier.DeanoAbr/Configuration/EmailSettings.cs`.**

```csharp
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
```

**Task 1.3 — `SportsResultsNotifier.DeanoAbr/Configuration/ScraperSettings.cs`.**

```csharp
namespace SportsResultsNotifier.DeanoAbr.Configuration;

public class ScraperSettings
{
    public const string SectionName = "Scraper";
    public string Url { get; set; } = "https://www.basketball-reference.com/boxscores/";
}
```

Verify: `dotnet build` succeeds.

---

### Phase 2 — Formatter (TDD)

**Task 2.1 — Write the failing test.**
`SportsResultsNotifier.DeanoAbr.Tests/FormatterServiceTests.cs`:

```csharp
using SportsResultsNotifier.DeanoAbr.Models;
using SportsResultsNotifier.DeanoAbr.Services;

namespace SportsResultsNotifier.DeanoAbr.Tests;

public class FormatterServiceTests
{
    [Fact]
    public void BuildBody_WithGames_FormatsHeaderAndEachLine()
    {
        var svc = new FormatterService();
        var games = new List<GameResult>
        {
            new("Milwaukee Bucks", 118, "Los Angeles Lakers", 117),
            new("Boston Celtics", 101, "Miami Heat", 99),
        };

        var body = svc.BuildBody(games, new DateOnly(2026, 1, 15));

        Assert.Contains("NBA Game Results - 2026-01-15", body);
        Assert.Contains("Milwaukee Bucks 118 - 117 Los Angeles Lakers", body);
        Assert.Contains("Boston Celtics 101 - 99 Miami Heat", body);
    }

    [Fact]
    public void BuildBody_WithNoGames_ReturnsNoGamesMessage()
    {
        var svc = new FormatterService();
        var body = svc.BuildBody(Array.Empty<GameResult>(), new DateOnly(2026, 1, 15));
        Assert.Contains("No games played", body);
    }
}
```

Run to confirm failure (class doesn't exist):
```bash
cd SportsResultsNotifier.DeanoAbr.Tests && dotnet test
```
Expected: compile error `The type or namespace name 'FormatterService' ... could not be found`.

**Task 2.2 — Implement `SportsResultsNotifier.DeanoAbr/Services/FormatterService.cs`.**

```csharp
using System.Text;
using SportsResultsNotifier.DeanoAbr.Models;

namespace SportsResultsNotifier.DeanoAbr.Services;

public class FormatterService
{
    public string BuildBody(IReadOnlyList<GameResult> games, DateOnly date)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"NBA Game Results - {date:yyyy-MM-dd}");
        sb.AppendLine(new string('-', 40));

        if (games.Count == 0)
        {
            sb.AppendLine("No games played.");
            return sb.ToString();
        }

        foreach (var game in games)
        {
            sb.AppendLine($"{game.Winner} {game.WinnerScore} - {game.LoserScore} {game.Loser}");
        }

        return sb.ToString();
    }
}
```

**Task 2.3 — Run tests + commit.**
```bash
dotnet test
```
Expected: `Passed! 2, Failed! 0`. Then:
```bash
cd .. && git add SportsResultsNotifier.DeanoAbr SportsResultsNotifier.DeanoAbr.Tests && git commit -m "Add formatter service for email body"
```

---

### Phase 3 — Scraper (TDD)

The scraper parses Basketball Reference's daily box-scores page. Each game is a
`<div class="game_summary ...">` containing a `<table class="teams">` with a
`<tr class="winner">` and `<tr class="loser">`; each row has the team link in
`td[1]` and the score in a `<td class="right">`. Parsing is a separate `Parse`
method so the test injects HTML without touching the network.

**Task 3.1 — Write the failing test.**
`SportsResultsNotifier.DeanoAbr.Tests/ScraperServiceTests.cs`:

```csharp
using Microsoft.Extensions.Logging.Abstractions;
using SportsResultsNotifier.DeanoAbr.Models;
using SportsResultsNotifier.DeanoAbr.Services;

namespace SportsResultsNotifier.DeanoAbr.Tests;

public class ScraperServiceTests
{
    private const string SampleHtml = """
<div class="game_summary expanded nohover">
  <table class="teams">
    <tbody>
      <tr class="winner">
        <td><a href="/teams/MIL/">Milwaukee Bucks</a></td>
        <td class="right">118</td>
      </tr>
      <tr class="loser">
        <td><a href="/teams/LAL/">Los Angeles Lakers</a></td>
        <td class="right">117</td>
      </tr>
    </tbody>
  </table>
</div>
<div class="game_summary expanded nohover">
  <table class="teams">
    <tbody>
      <tr class="winner">
        <td><a href="/teams/BOS/">Boston Celtics</a></td>
        <td class="right">101</td>
      </tr>
      <tr class="loser">
        <td><a href="/teams/MIA/">Miami Heat</a></td>
        <td class="right">99</td>
      </tr>
    </tbody>
  </table>
</div>
""";

    [Fact]
    public void Parse_WithGameSummaries_ExtractsWinnersLosersAndScores()
    {
        var scraper = new ScraperService(null!, NullLogger<ScraperService>.Instance);

        var games = scraper.Parse(SampleHtml);

        Assert.Equal(2, games.Count);
        Assert.Equal(new GameResult("Milwaukee Bucks", 118, "Los Angeles Lakers", 117), games[0]);
        Assert.Equal(new GameResult("Boston Celtics", 101, "Miami Heat", 99), games[1]);
    }

    [Fact]
    public void Parse_WithNoSummaries_ReturnsEmptyList()
    {
        var scraper = new ScraperService(null!, NullLogger<ScraperService>.Instance);

        var games = scraper.Parse("<html><body></body></html>");

        Assert.Empty(games);
    }
}
```

Note the raw-string literal must stay flush with column 0 (no indentation on the
`"""` line) or the sample HTML will be whitespace-trimmed incorrectly.

Run to confirm failure:
```bash
cd SportsResultsNotifier.DeanoAbr.Tests && dotnet test
```
Expected: compile error `The type or namespace name 'ScraperService' ... could not be found`.

**Task 3.2 — Implement the scraper.**
`SportsResultsNotifier.DeanoAbr/Services/IScraperService.cs`:

```csharp
using SportsResultsNotifier.DeanoAbr.Models;

namespace SportsResultsNotifier.DeanoAbr.Services;

public interface IScraperService
{
    Task<List<GameResult>> ScrapeAsync(string url, CancellationToken ct = default);
}
```

`SportsResultsNotifier.DeanoAbr/Services/ScraperService.cs`:

```csharp
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using SportsResultsNotifier.DeanoAbr.Models;

namespace SportsResultsNotifier.DeanoAbr.Services;

public class ScraperService : IScraperService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ScraperService> _logger;

    public ScraperService(IHttpClientFactory httpClientFactory, ILogger<ScraperService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<List<GameResult>> ScrapeAsync(string url, CancellationToken ct = default)
    {
        var html = await _httpClientFactory.CreateClient("BasketballReference").GetStringAsync(url, ct);
        return Parse(html);
    }

    public List<GameResult> Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var games = new List<GameResult>();
        var summaries = doc.DocumentNode.SelectNodes("//div[contains(@class,'game_summary')]");

        if (summaries is null)
        {
            _logger.LogWarning("No game summaries found on the page.");
            return games;
        }

        foreach (var summary in summaries)
        {
            var winnerRow = summary.SelectSingleNode(".//tr[contains(@class,'winner')]");
            var loserRow = summary.SelectSingleNode(".//tr[contains(@class,'loser')]");
            if (winnerRow is null || loserRow is null)
            {
                continue;
            }

            var winner = winnerRow.SelectSingleNode("./td[1]/a")?.InnerText.Trim();
            var loser = loserRow.SelectSingleNode("./td[1]/a")?.InnerText.Trim();
            var winnerScore = ParseScore(winnerRow.SelectSingleNode("./td[contains(@class,'right')]")?.InnerText);
            var loserScore = ParseScore(loserRow.SelectSingleNode("./td[contains(@class,'right')]")?.InnerText);

            if (winner is null || loser is null || winnerScore is null || loserScore is null)
            {
                continue;
            }

            games.Add(new GameResult(winner, winnerScore.Value, loser, loserScore.Value));
        }

        return games;
    }

    private static int? ParseScore(string? raw) =>
        int.TryParse(raw, out var score) ? score : null;
}
```

**Task 3.3 — Run tests + commit.**
```bash
dotnet test
```
Expected: `Passed! 4, Failed! 0` (2 formatter + 2 scraper). Then:
```bash
cd .. && git add SportsResultsNotifier.DeanoAbr SportsResultsNotifier.DeanoAbr.Tests && git commit -m "Add HtmlAgilityPack scraper for Basketball Reference"
```

---

### Phase 4 — Email service (thin, no unit test)

Email is wired but not unit-tested (SMTP needs a live server); correctness is
smoke-tested in Phase 6 against Papercut.

**Task 4.1 — `SportsResultsNotifier.DeanoAbr/Services/IEmailService.cs`.**

```csharp
namespace SportsResultsNotifier.DeanoAbr.Services;

public interface IEmailService
{
    Task SendAsync(string subject, string body, CancellationToken ct = default);
}
```

**Task 4.2 — `SportsResultsNotifier.DeanoAbr/Services/EmailService.cs`.**

```csharp
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using SportsResultsNotifier.DeanoAbr.Configuration;

namespace SportsResultsNotifier.DeanoAbr.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string subject, string body, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_settings.From));
        message.To.Add(MailboxAddress.Parse(_settings.To));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        _logger.LogInformation("Email sent to {To}", _settings.To);
    }
}
```

Note: `using Microsoft.Extensions.Logging;` is pulled in by the Worker SDK's
implicit usings — no explicit line needed.

---

### Phase 5 — Worker + host wiring

**Task 5.1 — Overwrite `SportsResultsNotifier.DeanoAbr/Worker.cs`.**

```csharp
using Microsoft.Extensions.Options;
using SportsResultsNotifier.DeanoAbr.Configuration;
using SportsResultsNotifier.DeanoAbr.Services;

namespace SportsResultsNotifier.DeanoAbr;

public class Worker : BackgroundService
{
    private readonly IScraperService _scraper;
    private readonly IEmailService _email;
    private readonly FormatterService _formatter;
    private readonly ScraperSettings _scraperSettings;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IScraperService scraper,
        IEmailService email,
        FormatterService formatter,
        IOptions<ScraperSettings> scraperSettings,
        ILogger<Worker> logger)
    {
        _scraper = scraper;
        _email = email;
        _formatter = formatter;
        _scraperSettings = scraperSettings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in daily run");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task RunOnceAsync(CancellationToken ct)
    {
        _logger.LogInformation("Scraping {Url}", _scraperSettings.Url);
        var games = await _scraper.ScrapeAsync(_scraperSettings.Url, ct);

        if (games.Count == 0)
        {
            _logger.LogWarning("No games scraped; skipping email.");
            return;
        }

        var body = _formatter.BuildBody(games, DateOnly.FromDateTime(DateTime.Now));
        await _email.SendAsync("Daily NBA Results", body, ct);
    }
}
```

**Task 5.2 — Overwrite `SportsResultsNotifier.DeanoAbr/Program.cs`.**

```csharp
using SportsResultsNotifier.DeanoAbr;
using SportsResultsNotifier.DeanoAbr.Configuration;
using SportsResultsNotifier.DeanoAbr.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));
builder.Services.Configure<ScraperSettings>(builder.Configuration.GetSection(ScraperSettings.SectionName));

builder.Services.AddHttpClient("BasketballReference", client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("SportsResultsNotifier/1.0 (personal learning project; contact: your-email@example.com)");
});

builder.Services.AddSingleton<IScraperService, ScraperService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<FormatterService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
```

**Task 5.3 — Overwrite `SportsResultsNotifier.DeanoAbr/appsettings.json`.**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Scraper": {
    "Url": "https://www.basketball-reference.com/boxscores/"
  },
  "Email": {
    "From": "your-email@example.com",
    "To": "recipient@example.com",
    "SmtpHost": "localhost",
    "SmtpPort": 25,
    "Username": "",
    "Password": ""
  }
}
```

**Task 5.4 — Build + commit.**
```bash
cd SportsResultsNotifier.DeanoAbr && dotnet build
cd .. && dotnet test
```
Expected: build succeeds, `Passed! 4, Failed! 0`. Then:
```bash
git add SportsResultsNotifier.DeanoAbr && git commit -m "Wire worker host, DI, and daily email loop"
```

---

### Phase 6 — Smoke test (real scrape + local email catch)

**Task 6.1 — Verify the live scrape without sending real email.**

Run with a fake SMTP target so no real mail is sent; the scrape + parse is what
we validate here. Set a local SMTP catcher (Papercut: `https://github.com/ChangemakerStudios/Papercut-SMTP`,
free, Windows) and point `SmtpHost` at `localhost:25`, or temporarily set an
unreachable host and accept the logged failure after the scrape line:

```bash
cd SportsResultsNotifier.DeanoAbr
dotnet run
```

Expected in the console (the first run fires immediately on startup):
```
... Scraping https://www.basketball-reference.com/boxscores/
```
followed by either `Email sent to ...` (if Papercut is running) or an SMTP
connection error log. **The key assertion is the scrape log line appears with no
parse exception** — this proves the live page's selectors still match the
fixture's shape. Press `Ctrl+C` to stop.

If the scrape logs `No games scraped; skipping email` or a `NullReference`/
parse failure, the live page's markup has drifted from the fixture — adjust the
selectors in `ScraperService.Parse` and re-run until games are found.

**Task 6.2 — Commit any selector fixes.**
```bash
git add SportsResultsNotifier.DeanoAbr && git commit -m "Fix scraper selectors against live page"
```

---

### Phase 7 — README + submission hygiene

**Task 7.1 — `SportsResultsNotifier.DeanoAbr/README.md`** with a
requirement→file traceability table (academy reviewers look for this):

```markdown
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
```

**Task 7.2 — Verify the PR diff is clean.**

```bash
cd "C:/Users/deant/OneDrive/Dokumentumok/GitHub/CodeReviews.Console.SportsResults"
git status --short
git diff --name-status main submission
```

Expected: only `A` (added) entries under `SportsResultsNotifier.DeanoAbr/` (and
`SportsResultsNotifier.DeanoAbr.Tests/`). No modification to the root `.gitignore`
or any `.github/` path. If `.gitignore` shows as modified, restore it:
`git checkout main -- .gitignore`.

**Task 7.3 — Final commit.**
```bash
git add SportsResultsNotifier.DeanoAbr && git commit -m "Add README with requirements traceability"
```

---

## Tests / validation summary

| Command | Expected result |
|---|---|
| `dotnet build` | `Build succeeded` |
| `dotnet test` | `Passed! 4, Failed! 0` |
| `dotnet run` (Phase 6) | `Scraping .../boxscores/` log line, no parse exception |
| `git diff --name-status main submission` | only `A` under `SportsResultsNotifier.DeanoAbr/` |

TDD cycle is followed per task in Phases 2 and 3: write failing test → confirm
failure → implement → confirm pass → commit.

## Risks, tradeoffs, open questions

- **Basketball Reference drift / anti-bot.** The site can change markup or rate
  limit. The fixture test pins the expected shape; the Phase 6 live smoke test
  catches real drift. A descriptive `User-Agent` is set. If blocked, add a small
  retry delay or reconsider the source (the requirement fixes this site).
- **24h loop, not a fixed wall-clock time.** The worker fires on startup then
  every 24h, so the send time drifts with service restarts. Acceptable for the
  requirement ("once a day"). A precise schedule (e.g. Cronos/NCrontab) is an
  easy later enhancement — open question for the user.
- **Credentials.** The email password must not be committed; the plan uses
  user-secrets/Papercut. Real sending is the user's configuration step.
- **`UserAgent` contact address** in `Program.cs` is a placeholder — user should
  replace it.
- **Push/PR are user-run** (this host cannot push to GitHub). Handoff: user runs
  `git push -u origin submission`, then opens a PR `DeanoAbr:submission` →
  `the-csharp-academy:main`, confirming only own commits appear.
