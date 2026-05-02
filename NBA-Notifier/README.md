# NBA Email Notifier
![Static Badge](https://img.shields.io/badge/.NET%20Version-10.0-orange?style=flat-square)  

# Purpose of The App
This is a showcase of a C# app that scrapes current NBA and WNBA information, such as games or conference standings, and sends emails to a list of subscribers containing that information. While the app could theoretically be configured to send out real emails, it's currently configured to send out test emails, which can be seen using a local SMTP email tester
like [Papercut](https://github.com/ChangemakerStudios/Papercut-SMTP), or [Smtp4Dev](https://github.com/rnwood/smtp4dev), which is what was used during development.  

# Basic Features
* The app scraps information from [Basketball-reference.com](https://www.basketball-reference.com/boxscores/) using [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack).  
* When passing "--refresh-snapshots" to the app as a program argument, the app will download local snapshots of the current NBA and WNBA information, meaning you can easily test emails during development without constantly scraping the website.  
* A simple "UseSnapshot" configuration in the AppSettings.json file tells the app to scrape data from either the local snapshots or the live website.    
* Emails are built using [RazorLight](https://www.nuget.org/packages/razorlight) and use HTML for the basic layout, with CSS for more visual details.  
* All CSS is built in-line with the HTML elements to avoid being stripped by email clients.  
* Emails are personalised to the subscriber, meaning each email is personal. This also applies to WNBA preferences, so that only subscribers who subscribe to the WNBA will receive updates.  
* Subscribers are kept in an SQLite database using Entity Framework Core, and each record contains the individual subscriber's name, email, and subscription preferences.  
* Email settings are kept in an AppSettings.json file, meaning changing the SMTP port, authorisation details, sender details, or any other settings can be configured easily within the file.  

# Sample Email Screenshot
![Sample Email](https://i.imgur.com/8jYNWpn.png) 

# How To Use The App

## Scraping

### Refreshing Local Snapshots
After cloning the repo, you can run the app with the program argument "--refresh-snapshots" to scrape and download local screenshots of both the NBA and WNBA pages. This can be done either through the terminal or by editing your launch configurations.  
![terminal command](https://i.imgur.com/ifpLEKC.png)  
![launch configurations](https://i.imgur.com/XXZVClx.png)  
Afterwards, two files should appear in a TestData directory  
![TestData directory](https://i.imgur.com/zhskCXq.png)  

### Scraping the live website
To switch between scraping local snapshots and scraping the live website, all you need to do is toggle the "UseSnapshot" setting in the appSettings.json within the configurations directory.  
![UseSnapshot field](https://i.imgur.com/Puld8aT.png)  

## Sending Emails

### For Testing With Smtp4Dev
Make sure that the Smtp4Dev NuGet package is installed and running in the terminal. You should see the interface after opening localhost:5000 in your browser, or whichever localhost port you chose, if you chose to change smtp4dev's default settings.  
![smtp4dev web interface](https://i.imgur.com/zOuso9d.png)  

### For Testing Without SMTP4Dev
All of the SMTP details are customizable through the appSettings.json file in the configuration directory.  
![email settings](https://i.imgur.com/zL7PcP1.png)  

# Architecture Overview
* When running the app, the scraping service scrapes the HTML from either the local screenshots or from the web URLs specified in the appSettings.json file. After scraping the HTML document, the values are stored in different models and then passed back to the app orchestrator that called the service.  
* After scraping, the data gets passed through to the report service, which uses RazorLight to build a string containing HTML according to the template stored in the Templates directory.  
* Once the orchestrator has the HTML string, it gets passed to the email service, which builds it into a sendable email with an HTML body.  
* The constructed email is then finally sent out according to the email settings specified in the appSettings.json configuration file.  
* All services inherit from interfaces and are passed through dependency injection for more testable and maintainable code.  

# Resources Used  
[.NET (10.0)](https://learn.microsoft.com/en-us/dotnet/)  
[HtmlAgilityPack (1.12.4)](https://www.nuget.org/packages/HtmlAgilityPack/1.12.4)  
[MailKit (4.16.0)](https://www.nuget.org/packages/MailKit/4.16.0)  
[EntityFrameworkCore (10.0.7)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/10.0.7)  
[RazorLight (2.3.1)](https://www.nuget.org/packages/RazorLight/2.3.1)  
[MimeKit (4.16.0)](https://www.nuget.org/packages/MimeKit/4.16.0)  

# Personal Thoughts
I really enjoyed learning about scraping HTML, how to locate the different HTML elements, and how to scrape the values of those elements. I also liked learning the basics of how to use RazorLight and how to make an HTML page with inline CSS. Both scraping and creating something from HTML were fresh topics that I enjoyed learning a lot about. I had touched on
using MailKit and sending emails before, but I think I learned a little more this time, too, and it was a nice refresher from what I learned before. This was my first time using Smtp4Dev, but I really liked it. It had a nice interface, and it was easy to set up, and it had a lot of useful tools to see and test my emails. To be honest, I really liked this 
project, and it was nice to do something different from what I've been doing recently.
