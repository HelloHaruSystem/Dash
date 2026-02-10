using Dash.Application.Features.Authentication.DTOs;
using Dash.Domain.Common;

namespace Dash.Application.Features.Authentication.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Handles the business logic for creating a new user account.
    /// </summary>
    public Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, string? ipAddress, string? userAgent);

    /// <summary>
    /// Handles the business logic for validating credentials and logging in.
    /// </summary>
    public Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent);

    /// <summary>
    /// Handles the business logic for taking a refresh token if valid then
    /// revoke the current create new tokens and return a authresponse
    /// </summary>
    public Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, string? ipAddress, string? userAgent);
}
