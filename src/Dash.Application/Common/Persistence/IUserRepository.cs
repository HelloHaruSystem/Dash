using Dash.Domain.Entities;

namespace Dash.Application.Common.Persistence;

public interface IUserRepository
{
    // General fetching
    public Task<User?> GetByIdAsync(Guid id);
    public Task<User?> GetByEmailAsync(string email);
    public Task<User?> GetByUsernameAsync(string username);

    // Auth-specific checks
    public Task<bool> ExistsByEmailAsync(string email);
    public Task<bool> ExistsByUsernameAsync(string username);

    // Commands
    public Task AddAsync(User user);
    // Not async because EF Core tracks changes
    public void Update(User user);
    public void Delete(User user);

    // Save trigger
    public Task SaveChangesAsync();
}
