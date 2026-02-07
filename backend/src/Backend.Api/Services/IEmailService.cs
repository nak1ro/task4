namespace Backend.Api.Services;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
}
