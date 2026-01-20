using System.ComponentModel.DataAnnotations;

namespace Dash.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; private set; } = null!;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; private set; } = null!;

    [Required]
    public string PasswordHash { get; private set; } = null!;

    // Email Verification
    public bool EmailVerified { get; private set; } = false;
    public DateTime? EmailVerifiedAt { get; private set; }

    // Account status
    public bool IsActive { get; private set; } = true;

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? PasswordChangedAt { get; private set; }

    // Localization
    [MaxLength(50)]
    public string? Timezone { get; private set; }

    [MaxLength(10)]
    public string? PreferredLanguage { get; private set; } = "en";

    // Security
    public int FailedLoginAttempts { get; private set; } = 0;

    [MaxLength(45)] // IPv6 MaxLength
    public string? LastLoginIp { get; private set; }

    // Password Reset
    public string? PasswordResetTokenHash { get; private set; }
    public DateTime? PasswordResetExpiresAt { get; private set; }

    // Navigation properties
    public ICollection<LoginAttempt> LoginAttempts { get; private set; } = [];

    // Private constructor
    private User() { }

    /// <summary>
    /// Factory for standart public registration
    /// Required verification and starts as IsActive
    /// </summary>
    public static User Create(string username, string email, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
        };
    }
}
