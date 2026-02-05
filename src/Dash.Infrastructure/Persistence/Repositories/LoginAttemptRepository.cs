using Dash.Application.Common.Persistence;
using Dash.Domain.Entities;
using Dash.Infrastructure.Data;
using Microsoft.Extensions.Logging;

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
