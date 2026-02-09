namespace Dash.Application.Features.Authentication.DTOs;

public sealed record AuthResponse
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Token { get; init; }
    public required string Refreshtoken { get; init; }
}
