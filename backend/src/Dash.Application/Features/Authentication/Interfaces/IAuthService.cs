using Dash.Application.Features.Authentication.DTOs;
using Dash.Domain.Common;
using Dash.Domain.Entities;

namespace Dash.Application.Features.Authentication.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Handles the business logic for creating a new user account.
    /// </summary>
    public Task<Result<(AuthResponse Response, RefreshToken RefreshToken)>> RegisterAsync(RegisterRequest request, string? ipAddress, string? userAgent);

    /// <summary>
    /// Handles the business logic for validating credentials and logging in.
    /// </summary>
    public Task<Result<(AuthResponse Response, RefreshToken RefreshToken)>> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent);

    /// <summary>
    /// Handles the business logic for taking a refresh token if valid then
    /// revoke the current create new tokens and return a authresponse
    /// </summary>
    public Task<Result<(AuthResponse Response, RefreshToken RefreshToken)>> RefreshAsync(string refreshToken, string? ipAddress, string? userAgent);
}
