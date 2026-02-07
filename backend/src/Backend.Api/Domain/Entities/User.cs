using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Domain.Entities;

public enum UserStatus
{
    Unverified = 0,
    Active = 1,
    Blocked = 2
}

public class User
{
    public Guid Id { get; private set; }
    
    [Required]
    public string FirstName { get; private set; }
    
    [Required]
    public string LastName { get; private set; }
    
    [Required]
    public string Email { get; private set; }
    
    [Required]
    public string PasswordHash { get; private set; }
    
    public UserStatus Status { get; private set; }
    
    public DateTime? LastLoginTime { get; private set; }
    
    public DateTime RegistrationTime { get; private set; }
    
    public string? EmailConfirmationToken { get; private set; }

    private User() { } // EF Core constructor

    public static User Create(string firstName, string lastName, string email, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = passwordHash,
            Status = UserStatus.Unverified,
            RegistrationTime = DateTime.UtcNow,
            EmailConfirmationToken = Guid.NewGuid().ToString("N")
        };
    }

    public void Block()
    {
        Status = UserStatus.Blocked;
    }

    public void Unblock()
    {
        Status = UserStatus.Active;
    }

    public void ConfirmEmail()
    {
        Status = UserStatus.Active;
        EmailConfirmationToken = null;
    }

    public void RecordLogin()
    {
        LastLoginTime = DateTime.UtcNow;
    }
}
