using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using GBC_Ticketing_Group145.Models;

namespace GBC_Ticketing_Group145.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Event configuration
            modelBuilder.Entity<Event>()
                .Property(e => e.AvailableTickets)
                .HasDefaultValue(0);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany()
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem configuration
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Event)
                .WithMany()
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Attendee)
                .WithMany()
                .HasForeignKey(c => c.AttendeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasIndex(c => new { c.EventId, c.AttendeeId })
                .IsUnique();

            // Ticket configuration
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany()
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Attendee)
                .WithMany()
                .HasForeignKey(t => t.AttendeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Conference", Description = "Professional conferences and seminars" },
                new Category { Id = 2, Name = "Concert", Description = "Live music performances" },
                new Category { Id = 3, Name = "Workshop", Description = "Hands-on learning sessions" },
                new Category { Id = 4, Name = "Sports", Description = "Sporting events and tournaments" },
                new Category { Id = 5, Name = "Festival", Description = "Cultural and community festivals" }
            );
        }
    }
}