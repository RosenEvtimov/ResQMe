# ResQMe

ResQMe is an ASP.NET Core Razor MVC web application for adopting animals from shelters and rescue organisations. It provides a public-facing catalog of shelters and animals, authenticated user workflows for submitting adoption requests, and an admin area for managing species, breeds, shelters, animals, and adoption requests.

---

## Table of contents

- [Features](#features)
- [Architecture & Project layout](#architecture--project-layout)
- [Technologies](#technologies)
- [Prerequisites](#prerequisites)
- [Get started — run locally](#get-started--run-locally)
  - [1. Clone repository](#1-clone-repository)
  - [2. Configure connection string](#2-configure-connection-string)
  - [3. Restore packages](#3-restore-packages)
  - [4. Run EF Core migrations / create DB](#4-run-ef-core-migrations--create-db)
  - [5. Run the application](#5-run-the-application)
- [Seeded data & admin account](#seeded-data--admin-account)
- [User account](#user-account)
- [High-level feature details](#high-level-feature-details)
- [Developer notes & configuration](#developer-notes--configuration)
- [Common tasks](#common-tasks)
- [Contributing](#contributing)
- [License](#license)

---

## Features

- Authentication & Authorization
  - ASP.NET Core Identity with `Admin` and `User` roles.
  - Middleware ensures newly registered users are assigned the `User` role if no roles assigned.
- Animals
  - Public list of animals.
  - Detailed animal page with image, age, gender, breed/species, shelter info and adoption status.
  - Admin CRUD for animals. AJAX endpoint to fetch breeds by species.
- Shelters
  - Public list and detail pages for shelters (contact info, description, image).
  - Admin CRUD for shelters.
- Species & Breeds
  - Admin pages to manage species and breeds.
- Adoption Requests
  - Authenticated users can submit adoption requests for available animals.
  - Users can view their own adoption requests.
  - Admins can view/approve/reject/undo adoption requests and filter by status.
- Validation & UX
  - Data annotation validation and client-side validation (jQuery Validation).
  - Responsive UI using Bootstrap.
- Database & Seeding
  - EF Core + SQL Server. Migrations included with initial seed data for species, breeds, shelters, animals and Identity schema.

---

## Architecture & Project layout

- `ResQMe_Project/` — Web application (Controllers, Views, Program.cs, wwwroot static assets)
- `ResQMe.Data/` — EF Core DbContext, entity models and migrations
- `ResQMe.Services.Core/` — Business logic services (registered in DI in `Program.cs`)
- `ResQMe.ViewModels/` — View models used by controllers and views
- `Areas/Identity/` — Identity UI Razor Pages for registration/login/account management

Pattern: Controllers + Razor Views for the main app; Identity uses Razor Pages. Services are injected via DI and handle business rules and data access orchestration.

---

## Technologies

- .NET 8
- C# 12
- ASP.NET Core MVC + Razor Pages (Identity area)
- Entity Framework Core (SQL Server provider)
- ASP.NET Core Identity
- Bootstrap 5
- jQuery + jQuery Validation

---

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 (recommended) or VS Code + C# extension
- SQL Server or LocalDB
- Git

---

## Get started — run locally

Follow these steps to run ResQMe from a fresh clone.

### 1. Clone repository

git clone https://github.com/RosenEvtimov/ResQMe.git cd ResQMe

### 2. Configure connection string

Open the `appsettings.json` in the `ResQMe_Project` project and set `ConnectionStrings:DefaultConnection` to your SQL Server instance.

### 3. Restore packages

- Visual Studio: Right-click solution → Restore NuGet Packages (or build).
- dotnet CLI: `dotnet restore`

### 4. Run EF Core migrations / create DB

You can apply migrations to create the database and seed initial data:

- Visual Studio (Package Manager Console)
  - Tools → NuGet Package Manager → Package Manager Console
  - Run: `Update-Database` (ensure `Default project` is `ResQMe.Data` or specify `-Project ResQMe.Data -StartupProject ResQMe_Project`)

- dotnet CLI
  - Ensure `dotnet-ef` is installed: `dotnet tool install --global dotnet-ef` (if not already)
  - From solution root:
    ```bash
    dotnet ef database update --project ResQMe.Data --startup-project ResQMe_Project
    ```

Migrations are included under `ResQMe.Data/Migrations`. Running the migrations creates the Identity schema, application tables, and seeds species, breeds, shelters and example animals.

### 5. Run the application

- Visual Studio: Set `ResQMe_Project` as the startup project and press F5 or Ctrl+F5.
- dotnet CLI: dotnet run --project ResQMe_Project

Open the app on the displayed HTTPS URL (typically `https://localhost:5001` or the port assigned by your environment).

---

## Seeded data & admin account

On application startup `Program.cs` seeds roles and an admin account if they do not exist:

- Roles created: `Admin`, `User`
- Seeded Admin account:
  - Email: `admin@admin.com`
  - Password: `Admin123!`

Initial sample data (species, many breeds, two shelters and three sample animals) is inserted by the EF Core migrations.

---

## User account

- There is no User account being seeded:
  - When using the webapplication for the first time, create your own profile via the registration button and test the differences between Admin and User role authorization.

---

## High-level feature details

- Role assignment middleware:
  - The app contains a middleware that verifies authenticated users have a role; if not, it assigns the `User` role and refreshes sign-in.
- Unique constraints:
  - Species name is unique.
  - Breed name + SpeciesId is unique.
  - Shelter name + Address is unique.
  - AdoptionRequest enforces uniqueness for (UserId, AnimalId) to disallow duplicate requests.
- Client UI:
  - Navigation reflects role membership (admin-only links show only for `Admin` role).
  - Animal details show an "Adopt Me" button only to signed-in users in the `User` role, and only for animals that are not adopted yet.
- Admin pages:
  - Admin can manage species, breeds, shelters, animals and adoption requests (approve/reject).
- Validation:
  - Views use ViewModels with data annotations for server-side validation. Client-side validation uses jQuery Validation bundled in the project.

---

## Developer notes & configuration

- Identity options: `Program.cs` sets `options.User.RequireUniqueEmail = true`. Adjust if you want duplicate emails allowed (not recommended).
- Email confirmation: `RequireConfirmedAccount` is set to `false` for development convenience. For production, enable email confirmation and configure an email sender.
- To add third-party login providers (Google, Facebook, etc.), add their configuration in `Program.cs` and register apps with the provider.
- Static files: CSS and JS are under `wwwroot`. Bootstrap and jQuery are included in `wwwroot/lib/`.
- Services: Business logic lives in `ResQMe.Services.Core`. Services are registered in DI in `Program.cs`.

---

## Common tasks

- Add a new migration: dotnet ef migrations add <Name> --project ResQMe.Data --startup-project ResQMe_Project
- Apply migrations: dotnet ef database update --project ResQMe.Data --startup-project ResQMe_Project
- Create a user and assign role manually (PMC or code):
  - Use `UserManager` & `RoleManager` in a scoped context or via seeding code.
---

## Contributing

- Fork the repository and open a pull request.
- Keep changes small and scoped; add tests for new logic where practical.
- Follow the existing layered pattern: controllers are thin and use services; services contain business logic; data access via EF Core is encapsulated in the `ResQMe.Data` project.

---

## License

This project is licensed under the MIT License — see the `LICENSE` file for details.

Below is the full MIT License text you can place in a `LICENSE` file (replace the year and owner as appropriate):

MIT License

Copyright (c) 2026 ResQMe

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
