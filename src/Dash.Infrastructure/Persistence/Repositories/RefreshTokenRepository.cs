using Dash.Application.Common.Persistence;
using Dash.Domain.Entities;
using Dash.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Dash.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly DashDbContext _context;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(
            DashDbContext context,
            ILogger<RefreshTokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        _logger.LogDebug("Creating refresh token for User: {UserId} Expires: {ExpiresAt}",
                refreshToken.UserId, refreshToken.ExpiresAt);

        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        RefreshToken? fetchedToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == token);

        if (fetchedToken is null)
        {
            _logger.LogWarning("Failed to fetch Token: {TokenString}", token);
        }
        else
        {
            _logger.LogInformation("Fetched Token: {TokenString}", token);
        }

        return fetchedToken;
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensAsync(Guid userId)
    {
        _logger.LogDebug("Fetching all refresh tokens By ID: {UserId}", userId);
        return await _context.RefreshTokens
            .Where(token => token.UserId == userId)
            .OrderByDescending(token => token.ExpiresAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogDebug("Saving changes to database");
        int changes = await _context.SaveChangesAsync();
        _logger.LogDebug("Saved {ChangeCount} changes to database", changes);
    }
}
