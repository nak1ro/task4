using Backend.Api.Domain.Entities;

namespace Backend.Api.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByConfirmationTokenAsync(string token);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task DeleteRangeAsync(IEnumerable<User> users); // For bulk delete
    Task SaveChangesAsync();
}
