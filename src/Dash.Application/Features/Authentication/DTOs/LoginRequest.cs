using System.ComponentModel.DataAnnotations;

namespace Dash.Application.Features.Authentication.DTOs;

public sealed record LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}
