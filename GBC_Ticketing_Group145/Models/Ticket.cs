using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GBC_Ticketing_Group145.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }
        
        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [Required]
        public string AttendeeId { get; set; } = string.Empty;
        
        [ForeignKey("AttendeeId")]
        public ApplicationUser? Attendee { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? QRCode { get; set; }
        
        public bool IsUsed { get; set; } = false;
        
        [Range(0, 5)]
        public int? Rating { get; set; }
        
        [StringLength(1000)]
        public string? Review { get; set; }
    }
}
