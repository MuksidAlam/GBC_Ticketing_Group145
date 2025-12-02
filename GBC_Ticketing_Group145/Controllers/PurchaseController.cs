using GBC_Ticketing_Group145.Data;
using GBC_Ticketing_Group145.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System; // Required for DateTime.UtcNow
using System.Linq; // Required for LINQ queries

namespace GBC_Ticketing_Group145.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchase/Buy/5 (Retrieves the purchase form)
        public async Task<IActionResult> Buy(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null) return NotFound();
            
            if (@event.AvailableTickets <= 0)
            {
                TempData["Message"] = "Sorry, this event is sold out!";
                return RedirectToAction("Index", "Event");
            }

            var purchase = new Purchase 
            { 
                EventId = @event.Id, 
                Event = @event 
            };
            
            return View(purchase);
        }

        // POST: Purchase/Buy (Handles the purchase transaction)
    [HttpPost]
    [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy([Bind("EventId,GuestName,GuestEmail,Quantity")] Purchase purchase)
        {
            // If validation fails, we must reload the event to pass back to the view
            if (!ModelState.IsValid)
            {
                var @event = await _context.Events
                    .Include(e => e.Category)
                    .FirstOrDefaultAsync(e => e.Id == purchase.EventId);

                if (@event == null) return NotFound();
                
                purchase.Event = @event; 
                TempData["Message"] = "Please check required fields and try again.";
                return View(purchase);
            }

            // Start Transaction Logic
            try
            {
                var eventToUpdate = await _context.Events.FindAsync(purchase.EventId);
                
                if (eventToUpdate == null || eventToUpdate.AvailableTickets < purchase.Quantity)
                {
                    TempData["Message"] = "Purchase failed: Not enough tickets available or event not found.";
                    return RedirectToAction("Index", "Event");
                }

                // 1. Calculate and set metadata
                purchase.TotalCost = purchase.Quantity * eventToUpdate.TicketPrice;
                purchase.PurchaseDate = DateTime.UtcNow;

                // 2. CRITICAL STEP: Decrement Available Tickets (Assignment requirement)
                eventToUpdate.AvailableTickets -= purchase.Quantity;

                // 3. Save both the new purchase and the updated event record
                _context.Add(purchase);
                _context.Update(eventToUpdate);
                await _context.SaveChangesAsync();

                // 4. Success Feedback and Redirect
                TempData["Message"] = $"Success! {purchase.Quantity} tickets purchased for {eventToUpdate.Title}. Total cost: {purchase.TotalCost:C}";
                
                return RedirectToAction("Index", "Event"); // Redirect to the event list
                // return View("Confirmation", purchase); // Alternatively, redirect to your Confirmation view
            }
            catch (Exception ex)
            {
                TempData["Message"] = "An error occurred during the purchase process: " + ex.Message;
                return RedirectToAction("Index", "Event");
            }
        }

        // GET: Purchase/Checkout - Process cart checkout
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            // Get cart from session
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                TempData["Message"] = "Your cart is empty!";
                return RedirectToAction("Index", "Event");
            }

            var cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            if (cart == null || !cart.Any())
            {
                TempData["Message"] = "Your cart is empty!";
                return RedirectToAction("Index", "Event");
            }

            // Load event details
            var eventIds = cart.Select(c => c.EventId).ToList();
            var events = await _context.Events
                .Include(e => e.Category)
                .Where(e => eventIds.Contains(e.Id))
                .ToListAsync();

            // Attach events to cart items
            foreach (var item in cart)
            {
                item.Event = events.FirstOrDefault(e => e.Id == item.EventId);
            }

            return View(cart);
        }

        // POST: Purchase/ProcessCheckout - AJAX endpoint for cart checkout with modal response
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            // Get cart
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return Json(new { success = false, message = "Cart is empty" });
            }

            var cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            if (cart == null || !cart.Any())
            {
                return Json(new { success = false, message = "Cart is empty" });
            }

            try
            {
                decimal totalCost = 0;
                var ticketIds = new List<int>();

                // Process each cart item
                foreach (var cartItem in cart)
                {
                    var eventItem = await _context.Events.FindAsync(cartItem.EventId);
                    if (eventItem == null)
                    {
                        return Json(new { success = false, message = $"Event not found: {cartItem.EventId}" });
                    }

                    if (eventItem.AvailableTickets < cartItem.Quantity)
                    {
                        return Json(new { success = false, message = $"Not enough tickets for {eventItem.Title}" });
                    }

                    // Create purchase record
                    var purchase = new Purchase
                    {
                        EventId = eventItem.Id,
                        GuestName = request.GuestName ?? User.Identity?.Name ?? "Guest",
                        GuestEmail = request.GuestEmail ?? User.Identity?.Name ?? "guest@example.com",
                        Quantity = cartItem.Quantity,
                        TotalCost = cartItem.Quantity * eventItem.TicketPrice,
                        PurchaseDate = DateTime.UtcNow
                    };

                    // Create tickets with QR codes (using QRCodeService would be better)
                    for (int i = 0; i < cartItem.Quantity; i++)
                    {
                        var ticket = new Ticket
                        {
                            EventId = eventItem.Id,
                            AttendeeId = userId,
                            PurchasedAt = DateTime.UtcNow,
                            QRCode = $"TICKET-{eventItem.Id}-{userId}-{Guid.NewGuid()}"
                        };
                        _context.Tickets.Add(ticket);
                        await _context.SaveChangesAsync(); // Save to get ticket ID
                        ticketIds.Add(ticket.Id);
                    }

                    // Update available tickets
                    eventItem.AvailableTickets -= cartItem.Quantity;
                    _context.Update(eventItem);

                    // Save purchase
                    _context.Add(purchase);
                    totalCost += purchase.TotalCost;
                }

                await _context.SaveChangesAsync();

                // Clear cart
                HttpContext.Session.Remove("Cart");

                return Json(new { 
                    success = true, 
                    message = "Purchase successful!", 
                    totalCost = totalCost,
                    ticketCount = cart.Sum(c => c.Quantity),
                    ticketIds = ticketIds
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Purchase failed: " + ex.Message });
            }
        }
    }
}

// Request model for checkout
public class CheckoutRequest
{
    public string? GuestName { get; set; }
    public string? GuestEmail { get; set; }
}