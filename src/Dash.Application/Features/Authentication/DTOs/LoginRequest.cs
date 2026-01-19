using System.ComponentModel.DataAnnotations;

namespace Dash.Application.Features.Authentication.DTOs;

public sealed record LoginRequest
{
    [Required]
    [MaxLength(255)]
    // Can be email or username
    public required string Identifier { get; init; }

    [Required]
    public required string Password { get; init; }
}
