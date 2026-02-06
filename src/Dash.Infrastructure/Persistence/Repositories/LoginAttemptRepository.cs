using Dash.Application.Common.Persistence;
using Dash.Domain.Entities;
using Dash.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Dash.Infrastructure.Persistence.Repositories;

public sealed class LoginAttemptRepository : ILoginAttemptRepository
{
    private readonly DashDbContext _context;
    private readonly ILogger<LoginAttemptRepository> _logger;

    public LoginAttemptRepository(
        DashDbContext context,
        ILogger<LoginAttemptRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(LoginAttempt loginAttempt)
    {
        _logger.LogDebug("Recording login attempt for UserId: {UserId}, Success: {IsSuccessful}",
                loginAttempt.UserId, loginAttempt.IsSuccessful);

        await _context.LoginAttempts.AddAsync(loginAttempt);
    }

    public async Task<int> CountRecentFailedAttemptsAsync(Guid userId, DateTime since)
    {
        _logger.LogDebug("Counting failed attempts for UserId: {UserId} since: {Since}", userId, since);

        return await _context.LoginAttempts
            .CountAsync(attempt =>
                    attempt.UserId == userId &&
                    !attempt.IsSuccessful &&
                    attempt.AttemptedAt >= since);
    }

    public async Task<IEnumerable<LoginAttempt>> GetAttemptsAsync(int skip, int take)
    {
        _logger.LogDebug("Fetching all login attempts with Offset: {Offset} and Limit: {limit}", skip, take);

        return await _context.LoginAttempts
            .OrderByDescending(attempt => attempt.AttemptedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<LoginAttempt?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Fetching login attempt by ID: {LoginAttemptId}", id);
        return await _context.LoginAttempts.FindAsync(id);
    }

    public async Task<IEnumerable<LoginAttempt>> GetByUserIdAsync(Guid userId)
    {
        _logger.LogDebug("Fetching login attempts by UserId: {UserId}", userId);
        return await _context.LoginAttempts
            .Where(attempt => attempt.UserId == userId)
            .OrderByDescending(attempt => attempt.AttemptedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LoginAttempt>> GetFailedAttemptsAsync(int skip, int take)
    {
        _logger.LogDebug("Fetching failed login attempts with Offset: {Offset} and Limit: {Limit}", skip, take);

        return await _context.LoginAttempts
            .Where(attempt => !attempt.IsSuccessful)
            .OrderByDescending(attempt => attempt.AttemptedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogDebug("Saving changes to database");
        int changes = await _context.SaveChangesAsync();
        _logger.LogDebug("Saved {ChangeCount} changes to database", changes);
    }
}
