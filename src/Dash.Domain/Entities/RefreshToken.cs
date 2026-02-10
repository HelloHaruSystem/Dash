namespace Dash.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }

    public string Token { get; private set; } = null!;

    public Guid UserId { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

    public DateTime CreatedAt { get; private set; }

    public string? UserAgent { get; private set; }

    public string? IpAddress { get; private set; }

    // Navigation property
    public User User { get; private set; } = null!;

    private RefreshToken() { }

    /// <summary>
    /// Static factory to record a RefreshToken
    /// Only sets unique data
    /// </summary>
    public static RefreshToken Create(Guid userId, string token, string? ipAddress, string? userAgent, TimeSpan lifetime)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow + lifetime,
            RevokedAt = null,
            CreatedAt = DateTime.UtcNow,
            UserAgent = userAgent,
            IpAddress = ipAddress
        };
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
}
