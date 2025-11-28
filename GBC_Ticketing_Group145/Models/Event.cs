using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GBC_Ticketing_Group145.Models
{
    public class Event
    {
        // Primary Key
        public int Id { get; set; }

        [Required(ErrorMessage = "Event title is required.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        // Foreign Key
        [Required(ErrorMessage = "A category must be selected.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Navigation Property
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Event date and time is required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ticket price is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between $0.01 and $10,000.00")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Available tickets count is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Available tickets cannot be negative.")]
        [Display(Name = "Available Tickets")]
        public int AvailableTickets { get; set; }

        // Organizer
        [Required]
        public string OrganizerId { get; set; } = string.Empty;
        
        [ForeignKey("OrganizerId")]
        public ApplicationUser? Organizer { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Computed property for old field name compatibility
        [NotMapped]
        public decimal TicketPrice 
        { 
            get => Price; 
            set => Price = value; 
        }

        [NotMapped]
        public DateTime DateTime 
        { 
            get => EventDate; 
            set => EventDate = value; 
        }
    }
}