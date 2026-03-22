# SportsResultsNotifier.davetn657

## Overview

An application that scrapes basketball game data from the web and sends that information through email daily

## Requirements

- Should read sports data from a website and send data to a email
- Program should run automatically without interaction

## Technologies

- C#
- Agility HTML
- XUnit
- Papercut SMTP (personal testing)

## Setup

- Update appsettings.json with your email and password.
- Setup Papercut SMTP or any other SMTP Service
- If running unit tests, change appsettings.json in the specific projects sln

## Looking back

- This was a fun project that took a step from the usual console applications.
- I didn't really have any difficulties with this projects, except for setting up the testing suite. It was difficult to setup the tests because of the reliance on smtp services. I ended up importing the appsettings.json file and creating a mock smtp connection that would fail
