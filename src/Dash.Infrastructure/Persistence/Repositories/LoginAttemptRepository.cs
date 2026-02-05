using Dash.Application.Common.Persistence;
using Dash.Domain.Entities;

namespace Dash.Infrastructure.Persistence.Repositories;

public sealed class LoginAttemptRepository : ILoginAttemptRepository
{
    public Task AddAsync(LoginAttempt loginAttempt)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountRecentFailedAttemptsAsync(Guid userId, DateTime since)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LoginAttempt>> GetAttemptsAsync(int skip, int take)
    {
        throw new NotImplementedException();
    }

    public Task<LoginAttempt?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LoginAttempt>> GetByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LoginAttempt>> GetFailedAttemptsAsync(int skip, int take)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}
