using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GBC_Ticketing_Group145.Data;
using GBC_Ticketing_Group145.Models;
using GBC_Ticketing_Group145.Services;
using GBC_Ticketing_Group145.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GBC_Ticketing_Group145.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPdfService _pdfService;
    private readonly IQRCodeService _qrService;

    public DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IPdfService pdfService, IQRCodeService qrService)
    {
        _db = db;
        _userManager = userManager;
        _pdfService = pdfService;
        _qrService = qrService;
    }

    // GET: /Dashboard/MyTickets
    public async Task<IActionResult> MyTickets()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var tickets = await _db.Tickets
            .Include(t => t.Event)
            .Where(t => t.AttendeeId == user.Id)
            .OrderByDescending(t => t.PurchasedAt)
            .ToListAsync();

        return View(tickets);
    }

    // GET: /Dashboard/DownloadTicket/5
    public async Task<IActionResult> DownloadTicket(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var ticket = await _db.Tickets.Include(t => t.Event).FirstOrDefaultAsync(t => t.Id == id && t.AttendeeId == user.Id);
        if (ticket == null) return NotFound();

        // Ensure QR exists
        var qrBase64 = ticket.QRCode ?? _qrService.GenerateQRCode($"ticket:{ticket.Id}:{user.Id}");

        var pdfBytes = _pdfService.GenerateTicketPdf(ticket, user, qrBase64);

        var filename = $"ticket_{ticket.Id}.pdf";
        return File(pdfBytes, "application/pdf", filename);
    }

    // POST: /Dashboard/RateTicket
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RateTicket(int ticketId, int rating)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId && t.AttendeeId == user.Id);
        if (ticket == null) return NotFound();

        rating = Math.Clamp(rating, 0, 5);
        ticket.Rating = rating;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(MyTickets));
    }

    // GET: /Dashboard/MyEvents
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> MyEvents()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        // Get all events created by this organizer with ticket sales data
        var eventsWithSales = await _db.Events
            .Include(e => e.Category)
            .Where(e => e.OrganizerId == user.Id)
            .Select(e => new MyEventsViewModel
            {
                EventId = e.Id,
                Title = e.Title,
                Location = e.Location,
                EventDate = e.EventDate,
                Price = e.Price,
                AvailableTickets = e.AvailableTickets,
                CategoryName = e.Category != null ? e.Category.Name : "N/A",
                TotalTicketsSold = _db.Tickets.Count(t => t.EventId == e.Id),
                TotalRevenue = _db.Tickets.Where(t => t.EventId == e.Id).Sum(t => (decimal?)t.Price) ?? 0
            })
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();

        var viewModel = new MyEventsDashboardViewModel
        {
            Events = eventsWithSales,
            TotalEvents = eventsWithSales.Count,
            TotalTicketsSold = eventsWithSales.Sum(e => e.TotalTicketsSold),
            TotalRevenue = eventsWithSales.Sum(e => e.TotalRevenue)
        };

        return View(viewModel);
    }

    // GET: /Dashboard/PurchaseHistory
    public async Task<IActionResult> PurchaseHistory(DateTime? startDate, DateTime? endDate, int? categoryId, bool showOnlyPastEvents = false)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        // Get all tickets for the user
        var query = _db.Tickets
            .Include(t => t.Event)
            .ThenInclude(e => e!.Category)
            .Where(t => t.AttendeeId == user.Id);

        // Filter by date range if provided (convert to UTC for comparison)
        if (startDate.HasValue)
        {
            var startUtc = startDate.Value.ToUniversalTime();
            query = query.Where(t => t.PurchasedAt >= startUtc);
        }

        if (endDate.HasValue)
        {
            var endUtc = endDate.Value.AddDays(1).ToUniversalTime(); // Include entire end date
            query = query.Where(t => t.PurchasedAt < endUtc);
        }

        // Filter by category if provided
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(t => t.Event!.CategoryId == categoryId.Value);
        }

        var tickets = await query.OrderByDescending(t => t.PurchasedAt).ToListAsync();

        // Convert to view model and filter by past events if needed
        var now = DateTime.UtcNow;
        var purchaseHistory = tickets.Select(t => new PurchaseHistoryViewModel
        {
            TicketId = t.Id,
            EventTitle = t.Event?.Title ?? "Unknown Event",
            CategoryName = t.Event?.Category?.Name ?? "N/A",
            EventDate = t.Event?.EventDate ?? DateTime.MinValue,
            PurchasedAt = t.PurchasedAt,
            Price = t.Price,
            Location = t.Event?.Location ?? "N/A",
            Rating = t.Rating,
            IsPastEvent = t.Event != null && t.Event.EventDate < now
        }).ToList();

        // Apply past events filter
        if (showOnlyPastEvents)
        {
            purchaseHistory = purchaseHistory.Where(p => p.IsPastEvent).ToList();
        }

        // Get all categories for the filter dropdown
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();

        var viewModel = new PurchaseHistoryFilterViewModel
        {
            PurchaseHistory = purchaseHistory,
            StartDate = startDate,
            EndDate = endDate,
            CategoryId = categoryId,
            Categories = categories,
            ShowOnlyPastEvents = showOnlyPastEvents
        };

        return View(viewModel);
    }
}
