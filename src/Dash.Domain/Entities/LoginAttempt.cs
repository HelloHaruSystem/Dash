using System.ComponentModel.DataAnnotations;

namespace Dash.Domain.Entities;

public sealed class LoginAttempt
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime AttemptedAt { get; private set; } // Set by Database

    [MaxLength(45)]
    public string? IpAddress { get; private set; }

    [MaxLength(500)]
    public string? UserAgent { get; private set; }

    public bool IsSuccessful { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;

    private LoginAttempt() { }

    /// <summary>
    /// Static factory to record a login attempt
    /// Only sets unique data
    /// </summary>
    public static LoginAttempt Create(Guid userId, bool isSuccess, string? ipAddress, string? userAgent = null)
    {
        return new LoginAttempt
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IsSuccessful = isSuccess,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
    }
}
