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
            client.Timeout = 10000; // 10 seconds timeout for operations
            client.ConnectTimeout = 10000; // 10 seconds for connection

            // Connect with SSL or STARTTLS based on port
            int port = int.Parse(emailConfig["SmtpPort"]);
            var secureSocketOptions = port == 465 
                ? MailKit.Security.SecureSocketOptions.Auto 
                : MailKit.Security.SecureSocketOptions.StartTls;

            _logger.LogInformation($"Connecting to SMTP server {emailConfig["SmtpHost"]}:{port}...");
            await client.ConnectAsync(emailConfig["SmtpHost"], port, secureSocketOptions);
            _logger.LogInformation("Connected successfully.");

            // Authenticate if credentials are provided
            var smtpUser = emailConfig["SmtpUser"];
            var smtpPass = emailConfig["SmtpPass"];
            
            if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPass))
            {
                await client.AuthenticateAsync(smtpUser, smtpPass);
            }
            
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            _logger.LogInformation($"Confirmation email sent to {toEmail}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {toEmail}");
        }
    }
}
