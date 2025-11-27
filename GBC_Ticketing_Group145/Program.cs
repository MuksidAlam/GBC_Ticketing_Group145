using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using GBC_Ticketing_Group145.Data;
using GBC_Ticketing_Group145.Models;
using GBC_Ticketing_Group145.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Serilog;

// Bootstrap a minimal logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog((ctx, services, lc) => lc
        .WriteTo.Console()
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "GBC_Ticketing_Group145")
        .MinimumLevel.Information()
    );

    // Add services to the container
    builder.Services.AddControllersWithViews();

    // Database configuration
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Identity configuration
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false; // Disabled for development
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // Configure token lifespan for password reset (24 hours)
    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
        options.TokenLifespan = TimeSpan.FromHours(24);
    });

    // Authorization policies
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdmin", policy => policy.RequireRole(Roles.Admin));
        options.AddPolicy("RequireOrganizer", policy => policy.RequireRole(Roles.Organizer, Roles.Admin));
    });

    // Session configuration for shopping cart
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest
            : Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    });

    // Application services
    builder.Services.AddScoped<IQRCodeService, QRCodeService>();
    builder.Services.AddScoped<IPdfService, PdfService>();
    builder.Services.AddTransient<IEmailSender, EmailSender>();

    // Cookie settings
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
            ? Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest 
            : Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    });

    // Data Protection
    builder.Services.AddDataProtection();

    var app = builder.Build();

    // Seed database with roles and users
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            SeedData.Initialize(services).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred seeding the database.");
        }
    }

    // Pipeline configuration
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // Basic Content Security Policy header (adjust per app requirements)
    app.Use(async (context, next) =>
    {
        context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline' https:; style-src 'self' 'unsafe-inline' https:; img-src 'self' data:;";
        await next();
    });

    // Map status codes to a friendly status page (404/500)
    app.UseStatusCodePagesWithReExecute("/Home/Status/{0}");

    // 🛑 GLOBAL CULTURE FIX: Ensures C# treats all number/date inputs (like TicketPrice) as en-US format (using '.')
    var defaultCulture = new RequestCulture("en-US");
    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = defaultCulture,
        SupportedCultures = new[] { defaultCulture.Culture },
        SupportedUICultures = new[] { defaultCulture.UICulture }
    });

    app.UseRouting();
    app.UseSession(); // Enable session for shopping cart
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}