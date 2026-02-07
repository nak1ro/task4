using AutoMapper;
using Backend.Api.Data.Repositories;
using Backend.Api.Domain.Entities;
using Backend.Api.Dtos;

namespace Backend.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<List<UserDto>>(users);
    }

    // IMPORTANT: Blocks multiple users at once.
    public async Task BlockUsersAsync(List<Guid> userIds)
    {
        foreach (var id in userIds)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.Block();
                await _userRepository.UpdateAsync(user);
            }
        }
        await _userRepository.SaveChangesAsync();
    }

    // NOTE: Unblocks users, allowing them to login again.
    public async Task UnblockUsersAsync(List<Guid> userIds)
    {
        foreach (var id in userIds)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.Unblock();
                await _userRepository.UpdateAsync(user);
            }
        }
        await _userRepository.SaveChangesAsync();
    }

    // NOTA BENE: Performs HARD delete. Users are removed from DB.
    public async Task DeleteUsersAsync(List<Guid> userIds)
    {
        var usersToDelete = new List<User>();
        foreach (var id in userIds)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                usersToDelete.Add(user);
            }
        }
        await _userRepository.DeleteRangeAsync(usersToDelete);
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeleteUnverifiedUsersAsync(List<Guid> userIds)
    {
        // Requirement implies user might select specific unverified users to delete.
        // If "Delete Unverified" button is a separate action to delete ALL unverified, 
        // the logic would differ.
        // User clarification: "8. selected users"
        // So this behaves same as DeleteUsersAsync but maybe frontend filters them?
        // Or backend validation ensures they are unverified?
        // Let's assume generic delete by ID, validation optional but safe.
        // But the button is "Delete unverified" specifically.
        // If I select mixed users and click "Delete unverified", should it delete only unverified ones?
        // Probably safer.
        
        var usersToDelete = new List<User>();
        foreach (var id in userIds)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null && user.Status == UserStatus.Unverified)
            {
                usersToDelete.Add(user);
            }
        }
        await _userRepository.DeleteRangeAsync(usersToDelete);
        await _userRepository.SaveChangesAsync();
    }
}
