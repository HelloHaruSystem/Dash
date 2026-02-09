using System.Security.Cryptography;

namespace Dash.Domain.Entities;

// TODO: refactor so that it doesn't contain too much logic
public class RefreshToken
{
    public Guid Id { get; private set; }

    public string Token { get; private set; } = null!;

    public Guid UserId { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

    public DateTime CreatedAt { get; private set; }

    public string? DeviceInfo { get; private set; }

    public string? IpAddress { get; private set; }

    // Navigation property
    public User User { get; private set; } = null!;

    private RefreshToken() { }

    /// <summary>
    /// Static factory to record a RefreshToken
    /// Only sets unique data
    /// </summary>
    public static RefreshToken Create(Guid userId, string? ipAddress, string? deviceInfo, TimeSpan lifetime)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow + lifetime,
            RevokedAt = null,
            CreatedAt = DateTime.UtcNow,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress
        };
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
}
