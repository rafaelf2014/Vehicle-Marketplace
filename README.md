# CliCar — Full-Stack Vehicle Marketplace

A full-stack vehicle marketplace web application built with **C# ASP.NET Core MVC** and **SQL Server**, developed as a team project. The platform supports the full lifecycle of vehicle listings — from publication and search to reservation and sale — with distinct experiences for buyers, sellers, and administrators.

---

## Features

### Buyers
- Browse and search vehicle listings with a rich filter panel — brand, model, category, fuel type, transmission, price range, year range, mileage, location, and condition (new/used)
- AJAX-driven dynamic search and model dropdowns (no full page reloads)
- Save and reload favourite search filters
- Add listings to favourites and manage them from a personal dashboard
- Reserve vehicles and track reservation status (Pending, Confirmed, Reserved)
- View personal reservation history and checkout details

### Sellers
- Create and manage vehicle listings with multi-image upload
- Edit vehicle details and manage images (add/remove individually)
- Soft-delete vehicles — marks as unavailable and cascades to deactivate associated listings
- Prevent deletion of vehicles with active reservations
- Sortable, paginated vehicle dashboard

### Admins
- Full admin dashboard with site visit analytics, sales charts, and platform-wide KPIs (total users, listings, vehicles, sales)
- User management — view all users by role (Buyer/Seller), block/unblock accounts, view individual profiles
- Listing management — monitor all listings and their states across the platform
- Vehicle management — overview of all vehicles and their owners
- Admin action history log — tracks all administrative actions with timestamp, target, and reason
- Site visit tracking middleware with session-based deduplication (5-minute window, filters assets and AJAX requests)

### General
- ASP.NET Identity with three roles: **Admin**, **Vendedor** (Seller), **Comprador** (Buyer)
- Email confirmation via SMTP (Mailtrap sandbox)
- Reservation conflict logic — prevents double-booking of the same vehicle
- Fully seeded database with brands, models, fuel types, vehicle classes, and Portuguese districts

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | C# / ASP.NET Core MVC |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Auth | ASP.NET Identity (role-based) |
| Frontend | Razor Views, AJAX, jQuery, HTML5, CSS |
| Email | SMTP via Mailtrap |
| File Storage | Local filesystem (`wwwroot/uploads/veiculos`) |

---

## Architecture

The project follows the **MVC** pattern with a service layer:

```
Controllers       — Handle HTTP requests and orchestrate responses
Services          — Business logic (VeiculoService, FileService, SmtpEmailSender)
Data              — EF Core DbContext, seed data, role seeding
Models/Classes    — Domain entities
Models/ViewModels — Typed view models for each page
Views             — Razor .cshtml templates with partial views
Middleware        — SiteVisitMiddleware for page view tracking
```

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (local or remote)
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository:
```bash
git clone https://github.com/rafaelf2014/Vehicle-Marketplace
cd Vehicle-Marketplace
```

2. Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=CliCarDb;Trusted_Connection=True;"
}
```

3. Apply migrations and seed the database:
```bash
dotnet ef database update
```

4. Run the application:
```bash
dotnet run
```

The database will be automatically seeded with roles, vehicle classes, fuel types, brands, models, and Portuguese district locations on first run.

---

## Project Structure

```
CliCarProject/
├── Controllers/         # MVC Controllers (Home, Car, Veiculos, Anuncios, Admin, User)
├── Data/                # DbContext, SeedData, SeedRoles
├── Middleware/          # SiteVisitMiddleware
├── Models/
│   ├── Classes/         # Domain entities (Veiculo, Anuncio, Reserva, ...)
│   └── ViewModels/      # Page-specific view models
├── Services/            # VeiculoService, FileService, SmtpEmailSender
├── Views/               # Razor views per controller
│   ├── Admin/
│   ├── Car/
│   ├── Veiculos/
│   ├── Anuncios/
│   └── Shared/
└── wwwroot/             # Static assets and uploaded vehicle images
```
