This is a sports results notifier, which is intended to run in the background and send daily emails at 11pm of basketball score updates in the NBA. 

To run, this expects you to set environment variables for the email address and password of the sender email account, as well as the recipient email address.

To do so, you can follow these instructions based on your current use:

The below are intended to be copied and pasted into the proper terminal, and replaced with your actual values.

**Windows (Command Prompt):**
cmdset EMAIL_FROM=you@gmail.com
set EMAIL_TO=recipient@example.com
set EMAIL_PASSWORD=your_app_password

**Windows (PowerShell, permanent):**
powershell[System.Environment]::SetEnvironmentVariable("EMAIL_FROM", "you@gmail.com", "User")
[System.Environment]::SetEnvironmentVariable("EMAIL_TO", "recipient@example.com", "User")
[System.Environment]::SetEnvironmentVariable("EMAIL_PASSWORD", "your_app_password", "User")

**Mac/Linux:**
bashexport EMAIL_FROM=you@gmail.com
export EMAIL_TO=recipient@example.com
export EMAIL_PASSWORD=your_app_password

If you are using gmail, you will need to have an app password setup for your account. These are the steps to do so:

- Go to your Google Account and open the Security tab
- Enable 2-Step Verification if it is not already on
- Search for "App Passwords" and create one for Mail
- Use the 16-character code as your EMAIL_PASSWORD

