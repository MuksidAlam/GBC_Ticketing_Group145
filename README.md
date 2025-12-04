# GBC Event Ticketing System - Group 145

A comprehensive event ticketing platform built with ASP.NET Core 9.0, featuring user authentication, event management, shopping cart, and ticket purchasing capabilities.

## 👥 Team Members
- **Al Muksid Alam Alvi**
- **Samuel**

## 🎯 Features

### Core Features
1. **✅ Enhanced Identity Core** - Complete user authentication with roles (Admin, Organizer, Attendee)
2. **✅ My Dashboard** - Personalized dashboard with ticket management
3. **✅ AJAX Live Search** - Real-time event search and filtering
4. **✅ AJAX Shopping Cart** - Session-based cart with live counter updates
5. **✅ AJAX Purchase Confirmation** - Modal-based purchase flow with animations
6. **✅ Global Error Handling** - Custom 404/500 pages with Serilog logging
7. **✅ Production Configuration** - Secure cookies, HTTPS, and hardened settings
8. **✅ Unit Testing** - Comprehensive xUnit test suite

### Advanced Features
9. **✅ Password Reset Flow** - Complete forgot/reset password with email integration
   - 24-hour token expiration
   - Professional email templates
   - Password strength indicator

10. **✅ My Events Dashboard** - Organizer event management portal
    - Revenue tracking per event
    - Sales analytics
    - Ticket availability monitoring

11. **✅ Purchase History** - Complete purchase tracking for users
    - Date range filtering
    - Category filtering
    - Rating system

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, jQuery, Custom CSS
- **Logging**: Serilog
- **Testing**: xUnit
- **Additional Libraries**:
  - QRCoder (QR code generation)
  - iText7 (PDF generation)
  - Npgsql (PostgreSQL provider)

## 📋 Prerequisites

- .NET 9.0 SDK
- PostgreSQL 12 or higher
- Visual Studio 2022 / VS Code / Rider

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/MuksidAlam/GBC_Ticketing_Group145.git
cd GBC_Ticketing_Group145
```

### 2. Database Setup

#### Update Connection String
Edit `appsettings.json` and `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gbc_ticketing_assignment1;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

#### Apply Migrations
```bash
cd GBC_Ticketing_Group145
dotnet ef database update
```

### 3. Configure Email (Optional)
For password reset functionality, configure SMTP in `appsettings.Development.json`:
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "User": "your-email@gmail.com",
    "Password": "your-app-password",
    "From": "noreply@gbcticketing.com"
  }
}
```

### 4. Run the Application
```bash
dotnet run
```

The application will be available at `http://localhost:5014`

## 👤 Default User Accounts

The system seeds the following accounts:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@gbc.ca | Admin@123 |
| Organizer | organizer@gbc.ca | Organizer@123 |

## 🧪 Running Tests

```bash
cd tests/GBC_Ticketing_Group145.Tests
dotnet test
```

Test Coverage:
- **14+ Unit Tests** covering:
  - Model validation
  - Controller actions
  - Business logic
  - Authentication flows

## 📁 Project Structure

```
GBC_Ticketing_Group145/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs
│   ├── CartController.cs
│   ├── DashboardController.cs
│   ├── EventController.cs
│   └── ...
├── Data/                 # Database context and seeding
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── Models/              # Domain models
│   ├── ApplicationUser.cs
│   ├── Event.cs
│   ├── Ticket.cs
│   └── ...
├── ViewModels/          # View models for UI
├── Services/            # Business logic services
│   ├── EmailSender.cs
│   ├── PdfService.cs
│   └── QRCodeService.cs
├── Views/               # Razor views
│   ├── Account/
│   ├── Dashboard/
│   ├── Event/
│   └── Shared/
├── wwwroot/            # Static files (CSS, JS, images)
└── Migrations/         # EF Core migrations

tests/
└── GBC_Ticketing_Group145.Tests/
    ├── Controllers/    # Controller tests
    ├── Models/         # Model tests
    └── TestUtilities/  # Test helpers
```

## 🎨 UI Features

### Professional Bootstrap 5 Theme
- Custom color scheme with gradients
- Responsive design for mobile devices
- Professional card-based layouts
- Smooth transitions and hover effects
- Consistent styling across all pages

### Color Palette
- Primary: `#2c3e50` (Deep blue-gray)
- Secondary: `#3498db` (Professional blue)
- Accent: `#e74c3c` (Alert red)
- Success: `#27ae60` (Success green)

## 🔐 Security Features

- **Password Requirements**: 8+ characters, uppercase, lowercase, digits
- **HTTPS Enforcement** in production
- **HttpOnly Cookies** for authentication
- **CSRF Protection** built-in
- **XSS Protection** via Razor encoding
- **Content Security Policy** headers
- **Session Security** with secure cookies

## 📊 Key Functionalities

### For Attendees
- Browse and search events
- Add tickets to cart
- Purchase tickets with confirmation
- View purchase history with filters
- Download ticket PDFs with QR codes
- Rate past events

### For Organizers
- Create and manage events
- View sales analytics
- Track revenue per event
- Monitor ticket availability
- Access performance metrics

### For Admins
- Full system access
- User management
- Category management
- System-wide analytics

## 🐛 Known Issues / Future Enhancements

- Email sending requires SMTP configuration (currently logs to console)
- Payment gateway integration pending
- Advanced analytics dashboard for organizers
- Profile management page

## 📝 Assignment Requirements Completed

✅ All core requirements met  
✅ Advanced features implemented  
✅ Professional UI with Bootstrap 5  
✅ Comprehensive testing  
✅ Production-ready configuration  
✅ Complete documentation  

## 📄 License

This project is developed as part of a college assignment.

## 🤝 Contributing

This is an academic project. For questions or issues, please contact the team members.

## 📞 Support

For technical support or questions:
- Open an issue on GitHub
- Contact team members via email

---

**© 2025 GBC Ticketing System - Group 145**
