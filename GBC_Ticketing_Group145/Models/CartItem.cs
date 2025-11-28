using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GBC_Ticketing_Group145.Models
{
    public class CartItem
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
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
