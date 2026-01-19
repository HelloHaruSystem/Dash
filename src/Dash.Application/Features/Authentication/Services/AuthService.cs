using Dash.Application.Features.Authentication.DTOs;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Domain.Common;

namespace Dash.Application.Features.Authentication.Services;

public sealed class AuthService : IAuthService
{
    public Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }
}
