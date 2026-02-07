using Backend.Api.Dtos;
using Backend.Api.Domain.Entities;

namespace Backend.Api.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(RegisterRequest request);
    Task<User> LoginAsync(LoginRequest request);
    Task ConfirmEmailAsync(string token);
}
