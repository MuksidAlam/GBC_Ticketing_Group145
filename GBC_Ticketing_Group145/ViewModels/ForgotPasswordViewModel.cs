using System.ComponentModel.DataAnnotations;

namespace GBC_Ticketing_Group145.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}
