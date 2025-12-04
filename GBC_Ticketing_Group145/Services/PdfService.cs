using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.Kernel.Geom;
using GBC_Ticketing_Group145.Models;

namespace GBC_Ticketing_Group145.Services;

public interface IPdfService
{
    byte[] GenerateTicketPdf(Ticket ticket, ApplicationUser user, string qrCodeBase64);
}

public class PdfService : IPdfService
{
    public byte[] GenerateTicketPdf(Ticket ticket, ApplicationUser user, string qrCodeBase64)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf, PageSize.A4);
        
        // Set margins
        document.SetMargins(40, 40, 40, 40);

        // Colors
        var primaryColor = new DeviceRgb(37, 99, 235); // Blue
        var secondaryColor = new DeviceRgb(59, 130, 246); // Light Blue
        var darkColor = new DeviceRgb(31, 41, 55); // Dark Gray
        var lightGrayColor = new DeviceRgb(243, 244, 246); // Light Gray
        var whiteColor = ColorConstants.WHITE;

        // ========== HEADER SECTION ==========
        var headerTable = new Table(1).UseAllAvailableWidth();
        headerTable.SetBackgroundColor(primaryColor);
        headerTable.SetBorder(Border.NO_BORDER);
        
        var headerCell = new Cell()
            .Add(new Paragraph("🎫 EVENT TICKET")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(28)
                .SetBold()
                .SetFontColor(whiteColor)
                .SetMarginTop(20)
                .SetMarginBottom(5))
            .Add(new Paragraph("Virtual Event Ticketing System")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(11)
                .SetFontColor(whiteColor)
                .SetMarginBottom(20))
            .SetBorder(Border.NO_BORDER)
            .SetBackgroundColor(primaryColor);
        
        headerTable.AddCell(headerCell);
        document.Add(headerTable);
        
        document.Add(new Paragraph("\n").SetMarginTop(0).SetMarginBottom(0));

        // ========== MAIN TICKET CARD ==========
        var mainTable = new Table(2, true).UseAllAvailableWidth();
        mainTable.SetBorder(new SolidBorder(primaryColor, 3));
        mainTable.SetBackgroundColor(whiteColor);

        // LEFT COLUMN - Event Details
        var leftColumn = new Cell()
            .SetBorder(Border.NO_BORDER)
            .SetPadding(20)
            .SetBackgroundColor(lightGrayColor);

        // Event Title
        leftColumn.Add(new Paragraph(ticket.Event?.Title ?? "Event")
            .SetFontSize(22)
            .SetBold()
            .SetFontColor(darkColor)
            .SetMarginBottom(15));

        // Category Badge
        if (ticket.Event?.Category?.Name != null && !string.IsNullOrEmpty(ticket.Event.Category.Name))
        {
            var categoryPara = new Paragraph($"📌 {ticket.Event.Category.Name}")
                .SetFontSize(11)
                .SetBackgroundColor(secondaryColor)
                .SetFontColor(whiteColor)
                .SetPadding(5)
                .SetBorderRadius(new BorderRadius(5))
                .SetMarginBottom(15)
                .SetWidth(UnitValue.CreatePointValue(120));
            leftColumn.Add(categoryPara);
        }

        // Date and Time Section
        leftColumn.Add(new Paragraph("📅 DATE & TIME")
            .SetFontSize(10)
            .SetBold()
            .SetFontColor(primaryColor)
            .SetMarginTop(10)
            .SetMarginBottom(5));
        
        leftColumn.Add(new Paragraph(ticket.Event?.EventDate.ToString("dddd, MMMM dd, yyyy") ?? "")
            .SetFontSize(13)
            .SetBold()
            .SetFontColor(darkColor)
            .SetMarginBottom(3));
        
        leftColumn.Add(new Paragraph(ticket.Event?.EventDate.ToString("hh:mm tt") ?? "")
            .SetFontSize(12)
            .SetFontColor(darkColor)
            .SetMarginBottom(15));

        // Location Section
        leftColumn.Add(new Paragraph("📍 LOCATION")
            .SetFontSize(10)
            .SetBold()
            .SetFontColor(primaryColor)
            .SetMarginTop(5)
            .SetMarginBottom(5));
        
        leftColumn.Add(new Paragraph(ticket.Event?.Location ?? "")
            .SetFontSize(12)
            .SetFontColor(darkColor)
            .SetMarginBottom(15));

        // Price Section
        leftColumn.Add(new Paragraph("💳 TICKET PRICE")
            .SetFontSize(10)
            .SetBold()
            .SetFontColor(primaryColor)
            .SetMarginTop(5)
            .SetMarginBottom(5));
        
        leftColumn.Add(new Paragraph($"${ticket.Price:F2}")
            .SetFontSize(20)
            .SetBold()
            .SetFontColor(new DeviceRgb(34, 197, 94)) // Green
            .SetMarginBottom(10));

        mainTable.AddCell(leftColumn);

        // RIGHT COLUMN - Attendee & QR Code
        var rightColumn = new Cell()
            .SetBorder(Border.NO_BORDER)
            .SetPadding(20)
            .SetBackgroundColor(whiteColor);

        // Attendee Section
        rightColumn.Add(new Paragraph("👤 ATTENDEE")
            .SetFontSize(10)
            .SetBold()
            .SetFontColor(primaryColor)
            .SetMarginBottom(8));

        var attendeeBox = new Paragraph()
            .SetBackgroundColor(lightGrayColor)
            .SetPadding(10)
            .SetBorderRadius(new BorderRadius(8))
            .SetMarginBottom(15);
        
        attendeeBox.Add(new Text($"{user.FullName}\n")
            .SetFontSize(13)
            .SetBold()
            .SetFontColor(darkColor));
        attendeeBox.Add(new Text($"{user.Email}\n")
            .SetFontSize(10)
            .SetFontColor(darkColor));
        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            attendeeBox.Add(new Text($"📞 {user.PhoneNumber}")
                .SetFontSize(10)
                .SetFontColor(darkColor));
        }
        
        rightColumn.Add(attendeeBox);

        // QR Code Section
        if (!string.IsNullOrEmpty(qrCodeBase64))
        {
            rightColumn.Add(new Paragraph("🔍 SCAN TO VERIFY")
                .SetFontSize(10)
                .SetBold()
                .SetFontColor(primaryColor)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(10)
                .SetMarginBottom(10));

            var qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
            var qrImage = new Image(ImageDataFactory.Create(qrCodeBytes));
            qrImage.SetWidth(140);
            qrImage.SetHeight(140);
            qrImage.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            qrImage.SetBorder(new SolidBorder(lightGrayColor, 3));
            qrImage.SetPadding(5);
            
            rightColumn.Add(qrImage);
        }

        // Ticket ID
        rightColumn.Add(new Paragraph($"\nTicket ID: #{ticket.Id}")
            .SetFontSize(9)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(darkColor)
            .SetMarginTop(10));

        mainTable.AddCell(rightColumn);
        mainTable.Complete();
        
        document.Add(mainTable);
        
        // ========== FOOTER SECTION ==========
        document.Add(new Paragraph("\n"));
        
        var footerTable = new Table(1).UseAllAvailableWidth();
        footerTable.SetBorder(Border.NO_BORDER);
        
        var footerCell = new Cell()
            .Add(new Paragraph($"Purchased on: {ticket.PurchasedAt:MMMM dd, yyyy 'at' hh:mm tt}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(9)
                .SetFontColor(darkColor)
                .SetMarginBottom(5))
            .Add(new Paragraph("⚠️ Please present this ticket (printed or digital) at the event entrance.")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10)
                .SetFontColor(darkColor)
                .SetBold()
                .SetMarginBottom(5))
            .Add(new Paragraph("For support, visit our website or contact the event organizer.")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(8)
                .SetFontColor(darkColor)
                .SetItalic()
                .SetMarginBottom(10))
            .SetBorder(Border.NO_BORDER)
            .SetBackgroundColor(lightGrayColor)
            .SetPadding(15);
        
        footerTable.AddCell(footerCell);
        document.Add(footerTable);

        // ========== TERMS & CONDITIONS ==========
        document.Add(new Paragraph("\n"));
        
        var termsText = new Paragraph("Terms & Conditions")
            .SetFontSize(9)
            .SetBold()
            .SetFontColor(darkColor)
            .SetMarginBottom(5);
        document.Add(termsText);
        
        var terms = new Paragraph()
            .SetFontSize(7)
            .SetFontColor(darkColor)
            .SetTextAlignment(TextAlignment.JUSTIFIED);
        
        terms.Add("• This ticket is non-refundable and non-transferable. ");
        terms.Add("• The organizer reserves the right to deny entry. ");
        terms.Add("• Lost or stolen tickets cannot be replaced. ");
        terms.Add("• Event details are subject to change. ");
        terms.Add("• By attending, you agree to follow all event rules and regulations.");
        
        document.Add(terms);

        document.Close();
        
        return memoryStream.ToArray();
    }
}
