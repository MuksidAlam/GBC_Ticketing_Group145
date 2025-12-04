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
    public class EventControllerTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_Returns_View_With_Event_List()
        {
            var context = CreateContext("Event_Index");
            context.Categories.Add(new Category { Id = 1, Name = "Category1" });
            context.Events.Add(new Event { Id = 10, Title = "E1", Price = 15m, AvailableTickets = 5, EventDate = System.DateTime.UtcNow, CategoryId = 1, Location = "L" });
            await context.SaveChangesAsync();

            var controller = new EventController(context);
            var result = await controller.Index(null, null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Event>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal(10, model.First().Id);
        }

        [Fact]
        public async Task SearchEvents_Returns_Partial_With_Filtered_Events()
        {
            // Arrange
            var context = CreateContext("Event_Search");
            context.Categories.Add(new Category { Id = 1, Name = "Music" });
            context.Categories.Add(new Category { Id = 2, Name = "Sports" });
            context.Events.Add(new Event 
            { 
                Id = 1, 
                Title = "Rock Concert", 
                Price = 50m, 
                AvailableTickets = 100, 
                EventDate = System.DateTime.UtcNow.AddDays(10), 
                CategoryId = 1, 
                Location = "Stadium",
                OrganizerId = "org1",
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            });
            context.Events.Add(new Event 
            { 
                Id = 2, 
                Title = "Jazz Night", 
                Price = 30m, 
                AvailableTickets = 50, 
                EventDate = System.DateTime.UtcNow.AddDays(5), 
                CategoryId = 1, 
                Location = "Club",
                OrganizerId = "org1",
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            });
            context.Events.Add(new Event 
            { 
                Id = 3, 
                Title = "Basketball Game", 
                Price = 25m, 
                AvailableTickets = 200, 
                EventDate = System.DateTime.UtcNow.AddDays(3), 
                CategoryId = 2, 
                Location = "Arena",
                OrganizerId = "org2",
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = new EventController(context);

            // Act - Search by title
            var result = await controller.SearchEvents("Rock", null, null);

            // Assert
            var partialResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_EventPartial", partialResult.ViewName);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Event>>(partialResult.Model);
            Assert.Single(model);
            Assert.Equal("Rock Concert", model.First().Title);
        }

        [Fact]
        public async Task SearchEvents_FilterByCategory_ReturnsFilteredResults()
        {
            // Arrange
            var context = CreateContext("Event_Search_Category");
            context.Categories.Add(new Category { Id = 1, Name = "Music" });
            context.Categories.Add(new Category { Id = 2, Name = "Sports" });
            context.Events.Add(new Event 
            { 
                Id = 1, 
                Title = "Event1", 
                Price = 50m, 
                AvailableTickets = 100, 
                EventDate = System.DateTime.UtcNow.AddDays(10), 
                CategoryId = 1, 
                Location = "Loc1",
                OrganizerId = "org1",
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            });
            context.Events.Add(new Event 
            { 
                Id = 2, 
                Title = "Event2", 
                Price = 30m, 
                AvailableTickets = 50, 
                EventDate = System.DateTime.UtcNow.AddDays(5), 
                CategoryId = 2, 
                Location = "Loc2",
                OrganizerId = "org1",
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = new EventController(context);

            // Act - Filter by Sports category
            var result = await controller.SearchEvents(null, 2, null);

            // Assert
            var partialResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Event>>(partialResult.Model);
            Assert.Single(model);
            Assert.Equal(2, model.First().CategoryId);
        }

        [Fact]
        public async Task SearchEvents_SortByPrice_ReturnsSortedResults()
        {
            // Arrange
            var context = CreateContext("Event_Search_Sort");
            context.Categories.Add(new Category { Id = 1, Name = "Music" });
            context.Events.Add(new Event 
            { 
                Id = 1, 
                Title = "Expensive Event", 
                Price = 100m, 
                AvailableTickets = 50, 
                EventDate = System.DateTime.UtcNow.AddDays(10), 
                CategoryId = 1, 
                Location = "Loc1",
                OrganizerId = "org1",
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            });
            context.Events.Add(new Event 
            { 
                Id = 2, 
                Title = "Cheap Event", 
                Price = 20m, 
                AvailableTickets = 100, 
                EventDate = System.DateTime.UtcNow.AddDays(5), 
                CategoryId = 1, 
                Location = "Loc2",
                OrganizerId = "org1",
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = new EventController(context);

            // Act - Sort by price ascending
            var result = await controller.SearchEvents(null, null, "Price");

            // Assert
            var partialResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Event>>(partialResult.Model);
            Assert.Equal(2, model.Count());
            Assert.Equal("Cheap Event", model.First().Title);
            Assert.Equal(20m, model.First().Price);
        }
    }
}
