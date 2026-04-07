# Sports Results Notifier
A C# 14/.NET 10 console application that scraps a sport (basketball) results website and send an email once per day with the retrieved results.

Created following the curriculum of [C# Academy](https://www.thecsharpacademy.com/)
[Sports Results Notifier](https://www.thecsharpacademy.com/project/19/sports-results)

## Features
- Has a web scraper service that retrieves daily basketball scores from the [Basketball Reference](https://www.basketball-reference.com/boxscores/) website, using [HtmlAgilityPack](https://html-agility-pack.net/)
- Has an email service that will email the daily basketball scores to a specified email address.
- All of this is orchestrated by a background service that is set to run once per day at 9 AM.
- Also has retry functionality for the web scraper and email services in case of connection issues.
- Implements Logging
- Unit Testing of the email service.

## Challenges Faced When Implementing This Project
- Wrapping my mind around web scraping took several days, probably because when I first started this project I had little knowledge about HTML and how to traverse the HTML structure of a web page. But once I got that basic knowledge down the web scraping concept became much easier.
- No other real challenges faced as I had implemented email functionality in a previous project and after reading a few articles about background services they were relatively easy to understand.

## What I Learned Implementing This Project
- Web scraping and html structure were the main things learned.
- Also learned about the importance of retrying when connecting to a resource such as a website and how if the connection may fail, depending on the reason for failure how you might want to retry again after a certain period of time.
- Learned about background services in .NET.

## Instructions To Send Email From This Application
- First off you will need a gmail account. Once inside of your gmail account click on the Google Account icon.
- Click on the button that says 'Manage your Google Account'.
- You will be taken to your Account options. Click on the option 'Security & sign-in' Click on and enable Two-Factor Authentication if it is not already enabled.
- After you have enabled Two-Factor Authentication, go to search at the top of the page and type in 'App Password'
- Click on 'App Password' in the results that are displayed from the drop down. From there you will be taken to a page to generate an app password.
- Simply type in the name for the App Password and click create and from there a pop up will display with your app password. Please copy this password so that you will have it for the next step.
- Next you need to set up the configuration files with the relevant information that will allow email to be sent from this application.
- The way the project is set up all information needed to send an email except for the subject and body are to be configured in the appsettings.json file (you can also use user secrets). In appsettings.json just fill in the relevant information:
```
"EmailConfiguration": {
    "SenderName": "Sender's name",
    "SenderEmail": "Sender's email (your gmail email address)",
    "ReceiverName": "Receiver's name",
    "ReceiverEmail": "Reciever's email (perhaps something like an alternate email)",
    "Password": "Your app password here",
    "Host": "smtp.gmail.com",
    "Port": 587
  }
```
- Then you can simply run the program:
- On Visual Studio and JetBrains Rider you can simply build and run the program from the IDE.
- With Visual Studio Code you can enter the following .NET CLI commands from the command line of your choice from the project directory:

```
dotnet build && dotnet run
```

## Areas To Improve Upon
- Continue to learn more and build more.

## Technolgies Used
- [Serilog](https://serilog.net/)
- [XUnit](https://xunit.net/?tabs=cs)
- [Moq](https://github.com/devlooped/moq)
- [MailKit](https://github.com/jstedfast/MailKit)
- [HtmlAgilityPack](https://html-agility-pack.net/)

## Helpful Resources Used
- [Background tasks with hosted services in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-10.0&tabs=visual-studio)
- [Using Background Services in ASP.NET Core](https://blog.wildermuth.com/2022/05/04/using-background-services-in-asp-net-core/)
- [Programming with Felipe Gavilan Sending Emails In ASP.NET With Gmail](https://www.youtube.com/watch?v=JX2bjkxnxTo&list=FL8cNANBvq4qCWiqtbFfaoTA&index=4)
- [Cancellation Tokens in C#: Best Practices for .NET Core Applications](https://www.nilebits.com/blog/2024/06/cancellation-tokens-in-csharp/)
- [How to Mock IOptions in ASP.NET Core](https://code-maze.com/csharp-mock-ioptions/)