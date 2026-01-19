using Dash.Application.Features.Authentication.DTOs;

namespace Dash.Application.Features.Authentication.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Handles the business logic for creating a new user account.
    /// </summary>
    public Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Handles the business logic for validating credentials and logging in.
    /// </summary>
    public Task<AuthResponse> LoginAsync(LoginRequest request);
}
