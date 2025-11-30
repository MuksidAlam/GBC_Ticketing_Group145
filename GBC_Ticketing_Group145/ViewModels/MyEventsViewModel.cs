namespace GBC_Ticketing_Group145.ViewModels;

public class MyEventsViewModel
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal Price { get; set; }
    public int AvailableTickets { get; set; }
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class MyEventsDashboardViewModel
{
    public List<MyEventsViewModel> Events { get; set; } = new();
    public int TotalEvents { get; set; }
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}
