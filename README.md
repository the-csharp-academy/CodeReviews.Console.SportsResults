# Sports Results Application

A C# / .NET program that emails the client recent NBA scores
This will send an email on a daily cron schedule - it runs at 09:00 (BST / GMT) every day, and the timezone can be changed via the app.config file.

## Application Details

- This is a .NET project that emails recent NBA Scores to a user.
- All usage details need to be entered in the applications app.config file, which is where credentials are stored.
- The application sends an email containing scores, which are scraped from: https://www.basketball-reference.com/boxscores/
- The application uses the SendGrid API to send emails.
- It uses the HtmlAgilityPack for scraping data from the basketball-reference page.

## Technologies used

- C# / .NET
- HTML Agility Pack - Package for web scraping.
- SendGrid / Twilio - For sending emails.
- Configuration Manager - For storing credentials and email details.
- Cronos - For scheduling the application to run at a specific time.

## Usage Instructions

1. Firstly, clone and open the repo.
2. The email functionality uses the SendGrid API for sending - You can generate an API key for free over on: https://sendgrid.com/en-us - you will also need to verify the sender address which you intend to send emails from.
3. Once done, open the app.config file and populate the API, sender and recipient details:
<img width="150" height="70" alt="image" src="https://github.com/user-attachments/assets/baf413aa-7d94-4490-b880-26fe50e59740" />

4. You can also change the timezone of the application here too (the default is 09:00 BST / GMT, which is UK / London time).

5. Run the application - an email will be received containing the results:
<img width="220" height="350" alt="image" src="https://github.com/user-attachments/assets/d97a7aad-c13f-4e36-a7ed-31535914919b" />

## Notes and Considerations
- I enjoyed using both the HTMLAgilityPack and SendGrid API for this project, both of which I hadn't used before.
- I opted to use the SendGrid API for this project (as opposed to creating SMTP objects) mainly for further exposure to using API's, as well as for a workaround for sending from Gmail accounts, which requires an app password to be created (as of 2022).
- Further enhancements could include customizing or "branding" the emails further, using HTML injection (within the emails, which is supported via the SendGrid API).

I'd like to thank the C# Academy for the opportunity to create this project, and I look forward to completing the next!
