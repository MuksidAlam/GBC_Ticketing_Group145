using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GBC_Ticketing_Group145.Models
{
    public class Purchase
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key to Event
        [Required]
        [Display(Name = "Event ID")]
        public int EventId { get; set; }

        // Navigation Property (needed for the Buy GET method to display event details)
        public Event? Event { get; set; }

        [Required(ErrorMessage = "Guest name is required.")]
        [StringLength(100)]
        [Display(Name = "Purchaser Name")]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Guest email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Display(Name = "Purchaser Email")]
        public string GuestEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 100, ErrorMessage = "You must purchase at least 1 ticket.")] // Limit the max quantity per purchase
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(18, 2)")] // Ensure precise currency storage in the database
        public decimal TotalCost { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
    }
}