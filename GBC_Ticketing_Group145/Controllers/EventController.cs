using GBC_Ticketing_Group145.Data;
using GBC_Ticketing_Group145.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Security.Claims;

namespace GBC_Ticketing_Group145.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Event/Index (Search, Filter, Sort)
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string searchString, 
            int? categoryId, 
            string sortOrder, 
            bool? showLowTickets) // Accepts nullable bool
        {
            var eventsQuery = _context.Events
                .Include(e => e.Category)
                .AsQueryable();

            // 1. Search by Title
            if (!String.IsNullOrEmpty(searchString))
            {
                eventsQuery = eventsQuery.Where(e => e.Title.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }

            // 2. Filter by Category
            if (categoryId.HasValue && categoryId.Value != 0)
            {
                eventsQuery = eventsQuery.Where(e => e.CategoryId == categoryId.Value);
            }
            
            // 3. Low-Ticket Alert Filter (Assignment Requirement)
            if (showLowTickets.HasValue && showLowTickets.Value)
            {
                eventsQuery = eventsQuery.Where(e => e.AvailableTickets < 5);
            }

            // 4. Sorting (ViewData for persisting sort links)
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            
            // Apply sorting logic
            eventsQuery = sortOrder switch
            {
                "title_desc" => eventsQuery.OrderByDescending(e => e.Title),
                "Date" => eventsQuery.OrderBy(e => e.DateTime),
                "date_desc" => eventsQuery.OrderByDescending(e => e.DateTime),
                "Price" => eventsQuery.OrderBy(e => e.TicketPrice),
                "price_desc" => eventsQuery.OrderByDescending(e => e.TicketPrice),
                _ => eventsQuery.OrderBy(e => e.Title), // Default sort
            };

            // Pass data for Filtering/Overview to the View
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", categoryId);
            ViewData["CurrentCategoryId"] = categoryId;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ShowLowTickets"] = showLowTickets; 
            ViewData["TotalEvents"] = await _context.Events.CountAsync(); 

            // Execute query and return view
            return View(await eventsQuery.ToListAsync());
        }

        // GET: Event/SearchEvents - AJAX endpoint for live search
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SearchEvents(
            string searchString, 
            int? categoryId, 
            string sortOrder)
        {
            var eventsQuery = _context.Events
                .Include(e => e.Category)
                .AsQueryable();

            // Search by Title
            if (!String.IsNullOrEmpty(searchString))
            {
                eventsQuery = eventsQuery.Where(e => e.Title.Contains(searchString));
            }

            // Filter by Category
            if (categoryId.HasValue && categoryId.Value != 0)
            {
                eventsQuery = eventsQuery.Where(e => e.CategoryId == categoryId.Value);
            }

            // Apply sorting logic
            eventsQuery = sortOrder switch
            {
                "title_desc" => eventsQuery.OrderByDescending(e => e.Title),
                "Date" => eventsQuery.OrderBy(e => e.DateTime),
                "date_desc" => eventsQuery.OrderByDescending(e => e.DateTime),
                "Price" => eventsQuery.OrderBy(e => e.TicketPrice),
                "price_desc" => eventsQuery.OrderByDescending(e => e.TicketPrice),
                _ => eventsQuery.OrderBy(e => e.Title),
            };

            var events = await eventsQuery.ToListAsync();
            return PartialView("_EventPartial", events);
        }

        // GET: Event/Create
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

    // POST: Event/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> Create([Bind("Title,Description,Location,CategoryId,EventDate,Price,AvailableTickets")] Event @event)
        {
            // Set organizer BEFORE validation (required field)
            if (User.Identity?.IsAuthenticated == true)
            {
                @event.OrganizerId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!;
            }
            
            // Set system fields
            @event.IsActive = true;
            @event.CreatedAt = DateTime.UtcNow;
            
            // Validate EventDate is not the default value
            if (@event.EventDate == DateTime.MinValue || @event.EventDate.Year < 2020)
            {
                ModelState.AddModelError("EventDate", "Event date and time is required.");
            }

            if (ModelState.IsValid)
            {
                @event.EventDate = DateTime.SpecifyKind(@event.EventDate, DateTimeKind.Local).ToUniversalTime();
                
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", @event.CategoryId);
            return View(@event);
        }

        // GET: Event/Edit/5
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();
            // Only Admins or the Organizer who created the event can edit
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole(Roles.Admin) && @event.OrganizerId != userId)
            {
                return Forbid();
            }
            
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", @event.CategoryId);
            return View(@event);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Location,CategoryId,EventDate,Price,AvailableTickets,IsActive,OrganizerId,CreatedAt")] Event @event)
        {
            if (id != @event.Id) return NotFound();

            // Validate EventDate is not the default value
            if (@event.EventDate == DateTime.MinValue || @event.EventDate.Year < 2020)
            {
                ModelState.AddModelError("EventDate", "Event date and time is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    @event.EventDate = DateTime.SpecifyKind(@event.EventDate, DateTimeKind.Local).ToUniversalTime();

                    // Ensure only Admin or original Organizer can update the event
                    var existing = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!User.IsInRole(Roles.Admin) && existing != null && existing.OrganizerId != userId)
                    {
                        return Forbid();
                    }

                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", @event.CategoryId);
            return View(@event);
        }

        // GET: Event/Delete/5
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (@event == null) return NotFound();
            // Only Admins or the Organizer who created the event can delete
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole(Roles.Admin) && @event.OrganizerId != userId)
            {
                return Forbid();
            }
            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!User.IsInRole(Roles.Admin) && @event.OrganizerId != userId)
                {
                    return Forbid();
                }

                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}