using System.ComponentModel.DataAnnotations;

namespace Dash.Application.Features.Authentication.DTOs;

public sealed record RefreshRequest
{
    [Required]
    public required string RefreshToken { get; init; }
}
