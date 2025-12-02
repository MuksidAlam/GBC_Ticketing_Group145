using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBC_Ticketing_Group145.Data;
using GBC_Ticketing_Group145.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GBC_Ticketing_Group145.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CartController> _logger;

    public CartController(ApplicationDbContext db, ILogger<CartController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET: /Cart/Index
    public async Task<IActionResult> Index()
    {
        var cartItems = GetCartItems();
        
        // Load event details for cart items
        var eventIds = cartItems.Select(c => c.EventId).ToList();
        var events = await _db.Events
            .Include(e => e.Category)
            .Where(e => eventIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, e => e);

        // Attach event details to cart items
        foreach (var item in cartItems)
        {
            if (events.TryGetValue(item.EventId, out var eventItem))
            {
                item.Event = eventItem;
            }
        }

        return View(cartItems);
    }

    // POST: /Cart/AddToCart - AJAX endpoint
    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] CartItemRequest request)
    {
        if (request == null || request.EventId <= 0 || request.Quantity <= 0)
        {
            return Json(new { success = false, message = "Invalid request data" });
        }

        // Check event availability
        var eventItem = await _db.Events.FindAsync(request.EventId);
        if (eventItem == null)
        {
            return Json(new { success = false, message = "Event not found" });
        }

        if (eventItem.AvailableTickets < request.Quantity)
        {
            return Json(new { success = false, message = $"Only {eventItem.AvailableTickets} tickets available" });
        }

        // Get or create cart
        var cart = GetCartItems();
        
        // Check if item already in cart
        var existingItem = cart.FirstOrDefault(c => c.EventId == request.EventId);
        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                EventId = request.EventId,
                Quantity = request.Quantity,
                UnitPrice = eventItem.TicketPrice
            });
        }

        // Save cart to session
        SaveCartItems(cart);

        var totalItems = cart.Sum(c => c.Quantity);
        var lowStockWarning = eventItem.AvailableTickets - request.Quantity < 10 
            ? $"Only {eventItem.AvailableTickets - request.Quantity} tickets left!" 
            : null;

        _logger.LogInformation("Item added to cart: EventId={EventId}, Quantity={Quantity}, User={User}", 
            request.EventId, request.Quantity, User.Identity?.Name);

        return Json(new { 
            success = true, 
            message = "Item added to cart successfully", 
            cartCount = totalItems,
            lowStockWarning = lowStockWarning
        });
    }

    // POST: /Cart/UpdateQuantity - AJAX endpoint
    [HttpPost]
    public IActionResult UpdateQuantity([FromBody] CartItemRequest request)
    {
        var cart = GetCartItems();
        var item = cart.FirstOrDefault(c => c.EventId == request.EventId);
        
        if (item == null)
        {
            return Json(new { success = false, message = "Item not found in cart" });
        }

        if (request.Quantity <= 0)
        {
            cart.Remove(item);
        }
        else
        {
            item.Quantity = request.Quantity;
        }

        SaveCartItems(cart);
        var totalItems = cart.Sum(c => c.Quantity);

        return Json(new { 
            success = true, 
            cartCount = totalItems,
            itemTotal = item?.Quantity * item?.UnitPrice ?? 0
        });
    }

    // POST: /Cart/RemoveItem - AJAX endpoint
    [HttpPost]
    public IActionResult RemoveItem([FromBody] CartItemRequest request)
    {
        var cart = GetCartItems();
        var item = cart.FirstOrDefault(c => c.EventId == request.EventId);
        
        if (item != null)
        {
            cart.Remove(item);
            SaveCartItems(cart);
        }

        var totalItems = cart.Sum(c => c.Quantity);

        return Json(new { 
            success = true, 
            message = "Item removed from cart",
            cartCount = totalItems
        });
    }

    // GET: /Cart/GetCartCount - AJAX endpoint
    [HttpGet]
    public IActionResult GetCartCount()
    {
        var cart = GetCartItems();
        var totalItems = cart.Sum(c => c.Quantity);
        return Json(new { cartCount = totalItems });
    }

    // POST: /Cart/ClearCart
    [HttpPost]
    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove("Cart");
        return RedirectToAction(nameof(Index));
    }

    // Helper methods
    private List<CartItem> GetCartItems()
    {
        var cartJson = HttpContext.Session.GetString("Cart");
        if (string.IsNullOrEmpty(cartJson))
        {
            return new List<CartItem>();
        }

        return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
    }

    private void SaveCartItems(List<CartItem> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("Cart", cartJson);
    }
}

// Request model for AJAX endpoints
public class CartItemRequest
{
    public int EventId { get; set; }
    public int Quantity { get; set; }
}
