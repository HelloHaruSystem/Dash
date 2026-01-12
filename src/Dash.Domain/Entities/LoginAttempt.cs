using System.ComponentModel.DataAnnotations;

namespace Dash.Domain.Entities;

public sealed class LoginAttempt
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(45)]
    public string IpAddress { get; set; } = null!;

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public bool IsSuccessful { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
