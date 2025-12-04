using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using GBC_Ticketing_Group145.Data;
using GBC_Ticketing_Group145.Controllers;
using GBC_Ticketing_Group145.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GBC_Ticketing_Group145.Tests.Controllers
{
    public class PurchaseControllerTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Buy_Post_Success_Reduces_AvailableTickets_And_Saves_Purchase()
        {
            var context = CreateContext("Buy_Post_Success");

            // Seed an event
            var ev = new Event { Id = 1, Title = "Test Event", Price = 10m, AvailableTickets = 10, EventDate = System.DateTime.UtcNow, CategoryId = 1, Location = "Loc" };
            context.Events.Add(ev);
            await context.SaveChangesAsync();

            var controller = new PurchaseController(context);
            // Provide TempData to avoid NullReference in controller catch blocks
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                new GBC_Ticketing_Group145.Tests.TestUtilities.TestTempDataProvider());

            var purchase = new Purchase { EventId = 1, GuestName = "John Doe", GuestEmail = "john@example.com", Quantity = 2 };

            var result = await controller.Buy(purchase);

            // After successful purchase, should redirect to Index
            Assert.IsType<RedirectToActionResult>(result);

            // Validate DB changes
            var updatedEvent = context.Events.First(e => e.Id == 1);
            Assert.Equal(8, updatedEvent.AvailableTickets);

            var savedPurchase = context.Purchases.FirstOrDefault(p => p.EventId == 1 && p.GuestEmail == "john@example.com");
            Assert.NotNull(savedPurchase);
            Assert.Equal(2, savedPurchase.Quantity);
            Assert.Equal(20m, savedPurchase.TotalCost);
        }

        [Fact]
        public async Task Buy_Post_Fails_When_Not_Enough_Tickets()
        {
            var context = CreateContext("Buy_Post_Fail");
            var ev = new Event { Id = 2, Title = "Sold Out", Price = 5m, AvailableTickets = 1, EventDate = System.DateTime.UtcNow, CategoryId = 1, Location = "Loc" };
            context.Events.Add(ev);
            await context.SaveChangesAsync();

            var controller = new PurchaseController(context);
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                new GBC_Ticketing_Group145.Tests.TestUtilities.TestTempDataProvider());
            var purchase = new Purchase { EventId = 2, GuestName = "Jane", GuestEmail = "jane@example.com", Quantity = 5 };

            var result = await controller.Buy(purchase);

            // Should redirect to Event Index when not enough tickets
            Assert.IsType<RedirectToActionResult>(result);
            var saved = context.Purchases.FirstOrDefault(p => p.EventId == 2 && p.GuestEmail == "jane@example.com");
            Assert.Null(saved);
        }
    }
}
