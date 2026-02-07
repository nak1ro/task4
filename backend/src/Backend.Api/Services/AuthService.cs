using AutoMapper;
using Backend.Api.Data.Repositories;
using Backend.Api.Domain.Entities;
using Backend.Api.Dtos;
using BCrypt.Net;

namespace Backend.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository, 
        IEmailService emailService, 
        IMapper mapper,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        // NOTA BENE: We do NOT check for email uniqueness here.
        // The database UNIQUE INDEX will handle it and throw an exception if duplicate.

        // 2. Create User Entity
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(request.FirstName, request.LastName, request.Email, passwordHash);

        // 3. Save to DB
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        // 4. Send Email
        // Construct link (assuming frontend route /confirm-email?token=...)
        string frontendUrl = _configuration["App:FrontendUrl"];
        string confirmationLink = $"{frontendUrl}/confirm-email?token={user.EmailConfirmationToken}";
        
        // Fire and forget email? Guideline says "asynchronously". 
        // We can just await it here for simplicity in this task context, or use `Task.Run`.
        // "the confirmation e-mail should be sent asynchronously" -> Task.Run to not block response?
        // But if SMTP fails, user might want to know? 
        // "Users are registered right away ... confirmation e-mail should be sent asynchronously"
        // Let's await it to ensure it sends, or catch and log.
        await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

        return user;
    }

    public async Task<User> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("Invalid email or password.");
        }

        if (user.Status == UserStatus.Blocked)
        {
            throw new Exception("Your account is blocked.");
        }

        bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!validPassword)
        {
            throw new Exception("Invalid email or password.");
        }

        user.RecordLogin();
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return user;
    }

    public async Task ConfirmEmailAsync(string token)
    {
        var user = await _userRepository.GetByConfirmationTokenAsync(token);
        if (user == null)
        {
            throw new Exception("Invalid confirmation token.");
        }

        user.ConfirmEmail();
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}
