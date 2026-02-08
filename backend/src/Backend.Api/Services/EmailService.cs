using Microsoft.Extensions.Configuration;
using Resend;

namespace Backend.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly IResend _resend;

    public EmailService(
        IConfiguration configuration, 
        ILogger<EmailService> logger,
        IResend resend)
    {
        _configuration = configuration;
        _logger = logger;
        _resend = resend;
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
    {
        try 
        {
            var emailMessage = new EmailMessage();
            emailMessage.From = _configuration["Resend:FromAddress"] ?? "onboarding@resend.dev";
            emailMessage.To.Add(toEmail);
            emailMessage.Subject = "Confirm your registration";
            emailMessage.HtmlBody = $@"
                <h2>Welcome to Task4 App!</h2>
                <p>Please confirm your registration by clicking the link below:</p>
                <a href='{confirmationLink}'>Confirm Registration</a>
            ";

            _logger.LogInformation($"Sending email using Resend to {toEmail}...");
            await _resend.EmailSendAsync(emailMessage);
            _logger.LogInformation($"Confirmation email sent to {toEmail} via Resend");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {toEmail}");
        }
    }
}
