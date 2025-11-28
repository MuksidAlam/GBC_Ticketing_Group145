using Microsoft.AspNetCore.Identity;
using GBC_Ticketing_Group145.Models;

namespace GBC_Ticketing_Group145.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Seed Roles
        string[] roleNames = { Roles.Admin, Roles.Organizer, Roles.Attendee };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed Admin User
        var adminEmail = "admin@gbc.ca";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }

        // Seed Sample Organizer
        var organizerEmail = "organizer@gbc.ca";
        var organizerUser = await userManager.FindByEmailAsync(organizerEmail);
        if (organizerUser == null)
        {
            var organizer = new ApplicationUser
            {
                UserName = organizerEmail,
                Email = organizerEmail,
                FirstName = "Event",
                LastName = "Organizer",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(organizer, "Organizer@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(organizer, Roles.Organizer);

                // Add sample events for this organizer
                if (!context.Events.Any())
                {
                    var categories = context.Categories.ToList();
                    var conferenceCategory = categories.FirstOrDefault(c => c.Name == "Conference");
                    var concertCategory = categories.FirstOrDefault(c => c.Name == "Concert");

                    var events = new List<Event>
                    {
                        new Event
                        {
                            Title = "Tech Conference 2025",
                            Description = "Join us for the biggest technology conference of the year featuring keynote speakers from leading tech companies.",
                            Location = "Metro Toronto Convention Centre",
                            CategoryId = conferenceCategory?.Id ?? 1,
                            EventDate = DateTime.UtcNow.AddMonths(2),
                            Price = 299.99m,
                            AvailableTickets = 500,
                            OrganizerId = organizer.Id,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Event
                        {
                            Title = "Summer Music Festival",
                            Description = "Experience an unforgettable night of live music with top artists from around the world.",
                            Location = "Budweiser Stage",
                            CategoryId = concertCategory?.Id ?? 2,
                            EventDate = DateTime.UtcNow.AddMonths(3),
                            Price = 89.99m,
                            AvailableTickets = 1000,
                            OrganizerId = organizer.Id,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    context.Events.AddRange(events);
                    await context.SaveChangesAsync();
                }
            }
        }

        // Seed Sample Attendee
        var attendeeEmail = "attendee@gbc.ca";
        var attendeeUser = await userManager.FindByEmailAsync(attendeeEmail);
        if (attendeeUser == null)
        {
            var attendee = new ApplicationUser
            {
                UserName = attendeeEmail,
                Email = attendeeEmail,
                FirstName = "Regular",
                LastName = "Attendee",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(attendee, "Attendee@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(attendee, Roles.Attendee);
            }
        }
    }
}
