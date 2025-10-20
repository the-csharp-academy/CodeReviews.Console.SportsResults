# MatchScoop

This project demonstrates website scraping and sending emails with .NET.

Runs as a background service, reading NBA match results from a website and sending game results report in an email once a day.

## Getting started

### Prerequisites

- .NET 9 SDK
- A Gmail account with 2-Step Verification enabled and an App Password generated (since Gmail blocks less secure apps).

### Installation

1. Clone the repository
    - `git clone https://github.com/nwdorian/MatchScoop.git`

2. Configure `appsettings.json`
    - update email settings
3. Navigate to project directory and run the project
    - `cd .\MatchScoop.Console\`
    - `dotnet run`

> [!NOTE]
> For testing purposes the email with results will be sent on startup.

### Gmail settings

- **UserEmail**: your Gmail address
- **Password**: The App Password generated in your Gmail account. (you can create it from your Google Account under Security > App Passwords).

> [!TIP]
> Use a different SMTP server by editing the *SmtpServer* and *Host* settings.

## Contributing

Contributions are welcome! Please fork the repository and create a pull request with your changes. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.

## Contact

For any questions or feedback, please open an issue.
