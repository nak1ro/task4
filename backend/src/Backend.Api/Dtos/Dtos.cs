using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Dtos;

public class RegisterRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public DateTime RegistrationTime { get; set; }
}

public class BulkActionRequest
{
    public List<Guid> UserIds { get; set; } = new();
}
