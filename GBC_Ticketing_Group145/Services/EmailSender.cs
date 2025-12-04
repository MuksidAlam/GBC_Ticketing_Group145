using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace GBC_Ticketing_Group145.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly IConfiguration _configuration;

    public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Read SMTP configuration from appsettings (optional)
        var smtpSection = _configuration.GetSection("Smtp");
        var host = smtpSection.GetValue<string>("Host");

        if (string.IsNullOrWhiteSpace(host))
        {
            // No SMTP configured — fall back to logging so development/test still works
            _logger.LogInformation("[EmailSender] SMTP not configured. Email to {Email}: {Subject}\n{Body}", email, subject, htmlMessage);
            return;
        }

        try
        {
            var port = smtpSection.GetValue<int?>("Port") ?? 25;
            var enableSsl = smtpSection.GetValue<bool?>("EnableSsl") ?? true;
            var user = smtpSection.GetValue<string>("User");
            var pass = smtpSection.GetValue<string>("Password");
            var from = smtpSection.GetValue<string>("From") ?? user ?? "no-reply@example.com";

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };

            if (!string.IsNullOrWhiteSpace(user))
            {
                client.Credentials = new NetworkCredential(user, pass);
            }

            using var message = new MailMessage(from, email, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
            _logger.LogInformation("[EmailSender] Sent email to {Email} via SMTP host {Host}", email, host);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EmailSender] Failed to send email to {Email}", email);
            // Fallback to logging the message body so confirmations can still be inspected in logs
            _logger.LogInformation("[EmailSender][Fallback] Email to {Email}: {Subject}\n{Body}", email, subject, htmlMessage);
        }
    }
}
