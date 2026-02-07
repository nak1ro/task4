using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Backend.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
    {
        try 
        {
            var emailConfig = _configuration.GetSection("Email");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailConfig["FromName"], emailConfig["FromAddress"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Confirm your registration";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Welcome to Task4 App!</h2>
                    <p>Please confirm your registration by clicking the link below:</p>
                    <a href='{confirmationLink}'>Confirm Registration</a>
                "
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(emailConfig["SmtpHost"], int.Parse(emailConfig["SmtpPort"]), false);
            
            // Note: For Mailhog (dev), no auth needed usually. 
            // For Prod, you'd add: await client.AuthenticateAsync(user, pass);
            
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            _logger.LogInformation($"Confirmation email sent to {toEmail}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {toEmail}");
            // Don't throw, just log. We don't want to break registration if email fails (or maybe we do? Requirement says "asynchronously", implying fire-and-forget or background job, but simplistic async/await is OK here for now)
        }
    }
}
