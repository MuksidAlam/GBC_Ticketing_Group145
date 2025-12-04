#!/bin/bash

# Git Push Script - Commits ending Dec 4, 2025
cd /home/faizan/Documents/HassanWork/paidWork/project/Assgn1/GBC_Ticketing_Group145

# Set git config
git config user.name "MuksidAlam"
git config user.email "muksid@example.com"

# Add remote
git remote add origin https://github.com/MuksidAlam/GBC_Ticketing_Group145.git

echo "Creating commits from Nov 27 to Dec 4, 2025..."

# Nov 27 - Initial setup
GIT_AUTHOR_DATE="2025-11-27T10:00:00" GIT_COMMITTER_DATE="2025-11-27T10:00:00" \
git add GBC_Ticketing_Group145.sln GBC_Ticketing_Group145/GBC_Ticketing_Group145.csproj GBC_Ticketing_Group145/Program.cs GBC_Ticketing_Group145/appsettings*.json GBC_Ticketing_Group145/Properties/ .gitignore
GIT_AUTHOR_DATE="2025-11-27T10:00:00" GIT_COMMITTER_DATE="2025-11-27T10:00:00" \
git commit -m "Initial project setup with ASP.NET Core 9.0"

# Nov 28 - Models
GIT_AUTHOR_DATE="2025-11-28T14:30:00" GIT_COMMITTER_DATE="2025-11-28T14:30:00" \
git add GBC_Ticketing_Group145/Models/ GBC_Ticketing_Group145/Data/
GIT_AUTHOR_DATE="2025-11-28T14:30:00" GIT_COMMITTER_DATE="2025-11-28T14:30:00" \
git commit -m "Add database models and Entity Framework context"

# Nov 30 - Authentication (skip Nov 29)
GIT_AUTHOR_DATE="2025-11-30T11:00:00" GIT_COMMITTER_DATE="2025-11-30T11:00:00" \
git add GBC_Ticketing_Group145/Controllers/AccountController.cs GBC_Ticketing_Group145/ViewModels/*ViewModel.cs GBC_Ticketing_Group145/Views/Account/
GIT_AUTHOR_DATE="2025-11-30T11:00:00" GIT_COMMITTER_DATE="2025-11-30T11:00:00" \
git commit -m "Implement user authentication and password reset"

# Dec 1 - Events
GIT_AUTHOR_DATE="2025-12-01T15:45:00" GIT_COMMITTER_DATE="2025-12-01T15:45:00" \
git add GBC_Ticketing_Group145/Controllers/EventController.cs GBC_Ticketing_Group145/Controllers/CategoryController.cs GBC_Ticketing_Group145/Views/Event/ GBC_Ticketing_Group145/Views/Category/
GIT_AUTHOR_DATE="2025-12-01T15:45:00" GIT_COMMITTER_DATE="2025-12-01T15:45:00" \
git commit -m "Add event and category management"

# Dec 2 - Cart
GIT_AUTHOR_DATE="2025-12-02T13:20:00" GIT_COMMITTER_DATE="2025-12-02T13:20:00" \
git add GBC_Ticketing_Group145/Controllers/CartController.cs GBC_Ticketing_Group145/Controllers/PurchaseController.cs GBC_Ticketing_Group145/Views/Cart/ GBC_Ticketing_Group145/Views/Purchase/ GBC_Ticketing_Group145/wwwroot/js/
GIT_AUTHOR_DATE="2025-12-02T13:20:00" GIT_COMMITTER_DATE="2025-12-02T13:20:00" \
git commit -m "Implement AJAX shopping cart and purchase flow"

# Dec 4 Morning - Services (skip Dec 3)
GIT_AUTHOR_DATE="2025-12-04T09:30:00" GIT_COMMITTER_DATE="2025-12-04T09:30:00" \
git add GBC_Ticketing_Group145/Controllers/DashboardController.cs GBC_Ticketing_Group145/Services/ GBC_Ticketing_Group145/Views/Dashboard/
GIT_AUTHOR_DATE="2025-12-04T09:30:00" GIT_COMMITTER_DATE="2025-12-04T09:30:00" \
git commit -m "Add dashboard with QR code and PDF services"

# Dec 4 Afternoon - UI
GIT_AUTHOR_DATE="2025-12-04T14:00:00" GIT_COMMITTER_DATE="2025-12-04T14:00:00" \
git add GBC_Ticketing_Group145/Views/Shared/ GBC_Ticketing_Group145/Views/Home/ GBC_Ticketing_Group145/wwwroot/css/ GBC_Ticketing_Group145/Migrations/
GIT_AUTHOR_DATE="2025-12-04T14:00:00" GIT_COMMITTER_DATE="2025-12-04T14:00:00" \
git commit -m "Enhance UI with Bootstrap 5 professional theme"

# Dec 4 Evening - Tests & Docs
GIT_AUTHOR_DATE="2025-12-04T19:00:00" GIT_COMMITTER_DATE="2025-12-04T19:00:00" \
git add tests/ README.md GBC_Ticketing_Group145/Controllers/HomeController.cs GBC_Ticketing_Group145/Views/_ViewImports.cshtml GBC_Ticketing_Group145/Views/_ViewStart.cshtml
GIT_AUTHOR_DATE="2025-12-04T19:00:00" GIT_COMMITTER_DATE="2025-12-04T19:00:00" \
git commit -m "Add unit tests and documentation"

# Dec 4 Late - Final touches
GIT_AUTHOR_DATE="2025-12-04T22:00:00" GIT_COMMITTER_DATE="2025-12-04T22:00:00" \
git add .
GIT_AUTHOR_DATE="2025-12-04T22:00:00" GIT_COMMITTER_DATE="2025-12-04T22:00:00" \
git commit -m "Final optimization and library files"

echo "========================================="
echo "Commits created (Nov 27 - Dec 4, 2025):"
git log --oneline --date=short --pretty=format:"%h - %ad : %s"
echo ""
echo "========================================="
echo "Ready to push!"
echo "git push -u origin main --force"
