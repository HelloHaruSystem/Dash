using System.ComponentModel.DataAnnotations;

namespace Dash.Domain.Entities;

public sealed class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    // Email Verification
    public bool EmailVerified { get; set; } = false;
    public DateTime? EmailVerifiedAt { get; set; }

    // Account status
    public bool IsActive { get; set; } = true;

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? PasswordChangedAt { get; set; }

    // Localization
    [MaxLength(50)]
    public string? Timezone { get; set; }

    [MaxLength(10)]
    public string? PreferredLanguage { get; set; } = "en";

    // Security
    public int FailedLoginAttempts { get; set; } = 0;

    [MaxLength(45)] // IPv6 MaxLength
    public string? LastLoginIp { get; set; }

    // Password Reset
    public string? PasswordResetTokenHash { get; set; }
    public DateTime? PasswordResetExpiresAt { get; set; }

    // Navigation properties
    public ICollection<LoginAttempt> LoginAttempts { get; set; } = [];
}
