using GBC_Ticketing_Group145.Models;

namespace GBC_Ticketing_Group145.ViewModels;

public class PurchaseHistoryViewModel
{
    public int TicketId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public DateTime PurchasedAt { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public bool IsPastEvent { get; set; }
}

public class PurchaseHistoryFilterViewModel
{
    public List<PurchaseHistoryViewModel> PurchaseHistory { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? CategoryId { get; set; }
    public List<Category> Categories { get; set; } = new();
    public bool ShowOnlyPastEvents { get; set; } = false;
}
