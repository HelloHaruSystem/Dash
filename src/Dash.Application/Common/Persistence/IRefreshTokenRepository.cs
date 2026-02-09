using Dash.Domain.Entities;

namespace Dash.Application.Common.Persistence;

public interface IRefreshTokenRepository
{
    // Fetching
    public Task<RefreshToken?> GetByTokenAsync(string token);
    public Task<IEnumerable<RefreshToken>> GetActiveTokensAsync(Guid userId);

    // Save Commands
    public Task CreateAsync(RefreshToken refreshToken);
    public Task RevokeAllByUserIdAsync(Guid userId);

    // Save
    public Task SaveChangesAsync();
}
