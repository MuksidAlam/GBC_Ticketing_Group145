# GBC Event Ticketing System

A comprehensive event ticketing platform built with ASP.NET Core 9.0, featuring event management, shopping cart, and ticket purchasing with QR code generation.

## Features

### User Features
- **User Authentication** - Secure registration and login with role-based access
- **Event Browsing** - Search and filter events in real-time with AJAX
- **Shopping Cart** - Add/remove tickets with live cart updates
- **Purchase Tickets** - Complete checkout flow with instant confirmations
- **Ticket Management** - Download tickets as PDF with QR codes
- **Purchase History** - Track and view past purchases with filtering

### Organizer Features
- **Event Management** - Create and manage events
- **Sales Dashboard** - View revenue and ticket sales analytics
- **Performance Metrics** - Track event performance

### Admin Features
- **Full System Access** - Manage users, events, and categories

## Technology Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, jQuery
- **Additional**: QRCoder, iText7 for PDF generation, Serilog logging

## Prerequisites

- .NET 9.0 SDK or higher
- PostgreSQL 12 or higher
- Git

## Local Setup

### 1. Clone Repository
```bash
git clone https://github.com/MuksidAlam/GBC_Ticketing_Group145.git
cd GBC_Ticketing_Group145
```

### 2. Configure Database Connection
Edit `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gbc_ticketing;Username=postgres;Password=yourpassword"
  }
}
```

### 3. Apply Database Migrations
```bash
cd GBC_Ticketing_Group145
dotnet ef database update
```

### 4. Run Application
```bash
dotnet run
```
### Testing

```bash
cd tests/GBC_Ticketing_Group145.Tests
dotnet test
```


