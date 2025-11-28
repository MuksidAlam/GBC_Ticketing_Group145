using System.ComponentModel.DataAnnotations;

namespace GBC_Ticketing_Group145.Models
{
    public class Category
    {
        // Primary Key
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50)]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        // Navigation property for the one-to-many relationship
        public ICollection<Event>? Events { get; set; }
    }
}