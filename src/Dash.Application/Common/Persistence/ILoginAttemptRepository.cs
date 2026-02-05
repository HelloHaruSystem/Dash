
using Dash.Domain.Entities;

namespace Dash.Application.Common.Persistence;

public interface ILoginAttemptRepository
{
    // General fetching
    public Task<LoginAttempt?> GetByIdAsync(Guid id);
    public Task<IEnumerable<LoginAttempt>> GetByUserIdAsync(Guid userId);
    public Task<IEnumerable<LoginAttempt>> GetFailedAttemptsAsync(int skip, int take);
    public Task<IEnumerable<LoginAttempt>> GetAttemptsAsync(int skip, int take);

    // Auth
    public Task<int> CountRecentFailedAttemptsAsync(Guid userId, DateTime since);

    // Commands
    public Task AddAsync(LoginAttempt loginAttempt);

    // Save trigger
    public Task SaveChangesAsync();
}

