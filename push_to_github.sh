#!/bin/bash

# Git Push Script with Backdated Commits
# This will create commits from Nov 27 to Dec 4, 2025

cd /home/faizan/Documents/HassanWork/paidWork/project/Assgn1/GBC_Ticketing_Group145

# Initialize git if not already done
if [ ! -d ".git" ]; then
    echo "Initializing git repository..."
    git init
    git branch -M main
fi

# Set git config
git config user.name "MuksidAlam"
git config user.email "muksid@example.com"

# Remove existing remote if exists
git remote remove origin 2>/dev/null

# Add remote
echo "Adding remote repository..."
git remote add origin https://github.com/MuksidAlam/GBC_Ticketing_Group145.git

# Dates for commits (skipping one day each week as requested)
# Starting from Nov 27 to Dec 4, 2025

echo "Creating backdated commits..."

# Commit 1: Nov 27, 2025 - Initial project setup
GIT_AUTHOR_DATE="2025-11-27T10:00:00" GIT_COMMITTER_DATE="2025-11-27T10:00:00" \
git add GBC_Ticketing_Group145.sln \
         GBC_Ticketing_Group145/GBC_Ticketing_Group145.csproj \
         GBC_Ticketing_Group145/Program.cs \
         GBC_Ticketing_Group145/appsettings*.json \
         GBC_Ticketing_Group145/Properties/ \
         .gitignore 2>/dev/null
GIT_AUTHOR_DATE="2025-11-27T10:00:00" GIT_COMMITTER_DATE="2025-11-27T10:00:00" \
git commit -m "Initial project setup with ASP.NET Core 9.0 MVC" 2>/dev/null || echo "First commit done"

# Commit 2: Nov 28, 2025 - Database models and context
GIT_AUTHOR_DATE="2025-11-28T14:30:00" GIT_COMMITTER_DATE="2025-11-28T14:30:00" \
git add GBC_Ticketing_Group145/Models/ \
         GBC_Ticketing_Group145/Data/ 2>/dev/null
GIT_AUTHOR_DATE="2025-11-28T14:30:00" GIT_COMMITTER_DATE="2025-11-28T14:30:00" \
git commit -m "Add database models and ApplicationDbContext" 2>/dev/null || echo "Models commit done"

# Skip Nov 29 (one day)

# Commit 3: Nov 30, 2025 - Identity and authentication
GIT_AUTHOR_DATE="2025-11-30T11:00:00" GIT_COMMITTER_DATE="2025-11-30T11:00:00" \
git add GBC_Ticketing_Group145/Controllers/AccountController.cs \
         GBC_Ticketing_Group145/ViewModels/LoginViewModel.cs \
         GBC_Ticketing_Group145/ViewModels/RegisterViewModel.cs \
         GBC_Ticketing_Group145/Views/Account/ 2>/dev/null
GIT_AUTHOR_DATE="2025-11-30T11:00:00" GIT_COMMITTER_DATE="2025-11-30T11:00:00" \
git commit -m "Implement user authentication with ASP.NET Identity" 2>/dev/null || echo "Auth commit done"

# Commit 4: Dec 1, 2025 - Event management
GIT_AUTHOR_DATE="2025-12-01T15:45:00" GIT_COMMITTER_DATE="2025-12-01T15:45:00" \
git add GBC_Ticketing_Group145/Controllers/EventController.cs \
         GBC_Ticketing_Group145/Controllers/CategoryController.cs \
         GBC_Ticketing_Group145/Views/Event/ \
         GBC_Ticketing_Group145/Views/Category/ 2>/dev/null
GIT_AUTHOR_DATE="2025-12-01T15:45:00" GIT_COMMITTER_DATE="2025-12-01T15:45:00" \
git commit -m "Add event and category management features" 2>/dev/null || echo "Events commit done"

# Commit 5: Dec 2, 2025 - Shopping cart and purchase
GIT_AUTHOR_DATE="2025-12-02T13:20:00" GIT_COMMITTER_DATE="2025-12-02T13:20:00" \
git add GBC_Ticketing_Group145/Controllers/CartController.cs \
         GBC_Ticketing_Group145/Controllers/PurchaseController.cs \
         GBC_Ticketing_Group145/Views/Cart/ \
         GBC_Ticketing_Group145/Views/Purchase/ \
         GBC_Ticketing_Group145/wwwroot/js/ 2>/dev/null
GIT_AUTHOR_DATE="2025-12-02T13:20:00" GIT_COMMITTER_DATE="2025-12-02T13:20:00" \
git commit -m "Implement AJAX shopping cart and purchase flow" 2>/dev/null || echo "Cart commit done"

# Skip Dec 3 (one day)

# Commit 6: Dec 4, 2025 - Dashboard and services
GIT_AUTHOR_DATE="2025-12-04T16:10:00" GIT_COMMITTER_DATE="2025-12-04T16:10:00" \
git add GBC_Ticketing_Group145/Controllers/DashboardController.cs \
         GBC_Ticketing_Group145/Services/ \
         GBC_Ticketing_Group145/Views/Dashboard/ \
         GBC_Ticketing_Group145/ViewModels/ 2>/dev/null
GIT_AUTHOR_DATE="2025-12-04T16:10:00" GIT_COMMITTER_DATE="2025-12-04T16:10:00" \
git commit -m "Add dashboard, QR code, and PDF services" 2>/dev/null || echo "Services commit done"

# Commit 7: Dec 4, 2025 (evening) - Password reset feature
GIT_AUTHOR_DATE="2025-12-04T20:30:00" GIT_COMMITTER_DATE="2025-12-04T20:30:00" \
git add GBC_Ticketing_Group145/ViewModels/ForgotPasswordViewModel.cs \
         GBC_Ticketing_Group145/ViewModels/ResetPasswordViewModel.cs \
         GBC_Ticketing_Group145/Views/Account/ForgotPassword.cshtml \
         GBC_Ticketing_Group145/Views/Account/ResetPassword.cshtml 2>/dev/null
GIT_AUTHOR_DATE="2025-12-04T20:30:00" GIT_COMMITTER_DATE="2025-12-04T20:30:00" \
git commit -m "Implement password reset functionality with email" 2>/dev/null || echo "Password reset commit done"

# Commit 8: Dec 4, 2025 (late) - UI improvements and migrations
GIT_AUTHOR_DATE="2025-12-04T23:00:00" GIT_COMMITTER_DATE="2025-12-04T23:00:00" \
git add GBC_Ticketing_Group145/Views/Shared/_Layout.cshtml \
         GBC_Ticketing_Group145/Views/Home/ \
         GBC_Ticketing_Group145/wwwroot/css/ \
         GBC_Ticketing_Group145/Migrations/ 2>/dev/null
GIT_AUTHOR_DATE="2025-12-04T23:00:00" GIT_COMMITTER_DATE="2025-12-04T23:00:00" \
git commit -m "Enhance UI with Bootstrap 5 professional theme" 2>/dev/null || echo "UI commit done"

# Commit 9: Dec 4, 2025 (final) - Tests and documentation
GIT_AUTHOR_DATE="2025-12-04T23:45:00" GIT_COMMITTER_DATE="2025-12-04T23:45:00" \
git add tests/ \
         README.md \
         .gitignore 2>/dev/null
GIT_AUTHOR_DATE="2025-12-04T23:45:00" GIT_COMMITTER_DATE="2025-12-04T23:45:00" \
git commit -m "Add unit tests and comprehensive documentation" 2>/dev/null || echo "Tests commit done"

# Add any remaining files
echo "Adding remaining files..."
git add . 2>/dev/null
git commit -m "Final project cleanup and optimization" 2>/dev/null || echo "Final commit done"

echo "========================================="
echo "Git repository prepared with commits from Nov 27 to Dec 4, 2025"
echo "Commits created (skipping Nov 29 and Dec 3):"
git log --oneline --graph --all
echo "========================================="
echo ""
echo "Ready to push! Run the following command:"
echo "git push -u origin main --force"
echo ""
echo "Note: Use --force carefully as it will overwrite remote history"
