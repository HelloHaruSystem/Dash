using Dash.Application.Common.Persistence;
using Dash.Domain.Entities;
using Dash.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dash.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly DashDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(DashDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Fetching user by ID: {UserId}", id);
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogDebug("Fetching user by Email: {Email}", email);
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        _logger.LogDebug("Fetching user by Username: {Username}", username);
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByIdentifierAsync(string identifier)
    {
        _logger.LogDebug("Fetching user by Identifier: {Identifier}", identifier);
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == identifier || u.Email == identifier);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        _logger.LogDebug("Checking if Email exists: {Email}", email);
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        _logger.LogDebug("Checking if Username exists: {Username}", username);
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        _logger.LogInformation("Adding new User: {Username}", user.Username);
        await _context.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _logger.LogInformation("Updating User: {Username}", user.Username);
        _context.Users.Update(user);
    }

    public void Delete(User user)
    {
        _logger.LogWarning("Deleting User: {Username}", user.Username);
        _context.Users.Remove(user);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogDebug("Saving changes to database");
        int changes = await _context.SaveChangesAsync();
        _logger.LogDebug("Saved {ChangeCount} changes to database", changes);
    }
}
