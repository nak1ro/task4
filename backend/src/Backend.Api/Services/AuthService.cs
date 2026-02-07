using AutoMapper;
using Backend.Api.Data.Repositories;
using Backend.Api.Domain.Entities;
using Backend.Api.Dtos;
using BCrypt.Net;
using Npgsql; // For PostgresException

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
        // 1. Transaction
        await _userRepository.BeginTransactionAsync();

        try
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
            string frontendUrl = _configuration["App:FrontendUrl"];
            string confirmationLink = $"{frontendUrl}/confirm-email?token={user.EmailConfirmationToken}";
            
            await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

            // 5. Commit
            await _userRepository.CommitTransactionAsync();

            return user;
        }
        catch (Exception ex)
        {
            await _userRepository.RollbackTransactionAsync();

            if (ex is Microsoft.EntityFrameworkCore.DbUpdateException dbEx && 
                dbEx.InnerException is Npgsql.PostgresException pgEx && 
                pgEx.SqlState == "23505") // Unique constraint violation
            {
                throw new Exception("Email is already registered.");
            }

            throw;
        }
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
        await _userRepository.BeginTransactionAsync();

        try
        {
            var user = await _userRepository.GetByConfirmationTokenAsync(token);
            if (user == null)
            {
                throw new Exception("Invalid confirmation token.");
            }

            user.ConfirmEmail();
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            await _userRepository.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _userRepository.RollbackTransactionAsync();
            throw;
        }
    }
}
