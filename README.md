# Plant Monitoring System

A simple web application to monitor plants and water them when neeeded

# Quick Start
The application can esily run by pulling the source code and run it directly with Visual Studio
- Access the solution by opening SVPlants.sln file
- It should work in Windows, MacOS, Visual Studio, VSCode, Rider

**Tested with:**
- Microsoft Visual Studio Community 2022 (64-bit) - Version 17.1.6
- Rider 2021.3.3


# Technologies:
## Backend - Web API
- .NET 6
- EF Core 6
- Mediator
- CQRS
- SQLite
- Clean Structure
- FluentValidation

## Frontend
- ReactJS
- Redux + Toolkit
- moment.js
- SweatAlert
- Bootstrap 5

# Usage

## Visible Alert
- A red "Need Water" status will be assigned to the plant without water for more than 6 hours

## Resting Plant
- "Last Watered At" will be assigned the current system date & time when the watering is stopped
- A progress bar shows how much resting time left

## Water Multiple Plants
- Select multiple rows and click "Water Multiple Plants" to water multiple plants at the same time

## Reset the application state
- Since we use SQLite as the storage, we need to delete 3 files of the database to reset the application state.
<img width="228" alt="image" src="https://user-images.githubusercontent.com/9654744/167286824-a05dcc28-7b60-4832-9fe8-6693678df902.png">

# Screenshot
<img width="1031" alt="image" src="https://user-images.githubusercontent.com/9654744/167286923-3b5dfc69-6931-4296-ad1b-4aaadc42c3ec.png">

## Animation
![watering](https://user-images.githubusercontent.com/9654744/167286893-f1354e78-b02c-482a-ad07-0c4655379fa8.gif)

