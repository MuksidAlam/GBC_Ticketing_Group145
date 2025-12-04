using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GBC_Ticketing_Group145.Controllers;
using GBC_Ticketing_Group145.Data;
using GBC_Ticketing_Group145.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Claims;

namespace GBC_Ticketing_Group145.Tests.Controllers
{
    public class CartControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed test data
            var category = new Category { Id = 1, Name = "Test Category" };
            var testEvent = new Event
            {
                Id = 1,
                Title = "Test Event",
                Description = "Test Description",
                Location = "Test Location",
                CategoryId = 1,
                Category = category,
                EventDate = System.DateTime.UtcNow.AddDays(7),
                TicketPrice = 50.00m,
                AvailableTickets = 100,
                OrganizerId = "test-organizer",
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            };

            context.Categories.Add(category);
            context.Events.Add(testEvent);
            context.SaveChanges();

            return context;
        }

        private CartController CreateControllerWithSession()
        {
            var context = GetInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<CartController>();
            var controller = new CartController(context, logger);

            // Setup HttpContext with Session
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            httpContext.Session = session;
            
            // Add user claims for authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "testuser@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            httpContext.User = principal;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public async Task AddToCart_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var controller = CreateControllerWithSession();
            var request = new CartItemRequest { EventId = 1, Quantity = 2 };

            // Act
            var result = await controller.AddToCart(request) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value as dynamic;
            Assert.NotNull(data);
            
            // Use reflection to check the anonymous type properties
            var successProp = data.GetType().GetProperty("success");
            var cartCountProp = data.GetType().GetProperty("cartCount");
            
            Assert.NotNull(successProp);
            Assert.NotNull(cartCountProp);
            Assert.True((bool)successProp.GetValue(data)!);
            Assert.Equal(2, (int)cartCountProp.GetValue(data)!);
        }

        [Fact]
        public async Task AddToCart_InvalidEventId_ReturnsError()
        {
            // Arrange
            var controller = CreateControllerWithSession();
            var request = new CartItemRequest { EventId = 999, Quantity = 1 };

            // Act
            var result = await controller.AddToCart(request) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value as dynamic;
            var successProp = data.GetType().GetProperty("success");
            Assert.False((bool)successProp.GetValue(data)!);
        }

        [Fact]
        public async Task AddToCart_InsufficientTickets_ReturnsError()
        {
            // Arrange
            var controller = CreateControllerWithSession();
            var request = new CartItemRequest { EventId = 1, Quantity = 200 }; // More than available

            // Act
            var result = await controller.AddToCart(request) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value as dynamic;
            var successProp = data.GetType().GetProperty("success");
            Assert.False((bool)successProp.GetValue(data)!);
        }

        [Fact]
        public void GetCartCount_EmptyCart_ReturnsZero()
        {
            // Arrange
            var controller = CreateControllerWithSession();

            // Act
            var result = controller.GetCartCount() as JsonResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value as dynamic;
            var cartCountProp = data.GetType().GetProperty("cartCount");
            Assert.Equal(0, (int)cartCountProp.GetValue(data)!);
        }

        [Fact]
        public async Task AddToCart_ShowsLowStockWarning_WhenBelow10()
        {
            // Arrange
            var controller = CreateControllerWithSession();
            var request = new CartItemRequest { EventId = 1, Quantity = 95 }; // Leaves 5 tickets

            // Act
            var result = await controller.AddToCart(request) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value as dynamic;
            var lowStockProp = data.GetType().GetProperty("lowStockWarning");
            Assert.NotNull(lowStockProp);
            var warningValue = lowStockProp.GetValue(data) as string;
            Assert.NotNull(warningValue);
            Assert.Contains("5 tickets left", warningValue);
        }
    }

    // Test session implementation
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

        public string Id => "test-session-id";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();

        public Task CommitAsync(System.Threading.CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task LoadAsync(System.Threading.CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[]? value)
            => _sessionStorage.TryGetValue(key, out value!);
    }
}
