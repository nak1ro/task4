using Backend.Api.Domain.Entities;
using Backend.Api.Dtos;

namespace Backend.Api.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task BlockUsersAsync(List<Guid> userIds);
    Task UnblockUsersAsync(List<Guid> userIds);
    Task DeleteUsersAsync(List<Guid> userIds);
    Task DeleteUnverifiedUsersAsync(List<Guid> userIds);
}
