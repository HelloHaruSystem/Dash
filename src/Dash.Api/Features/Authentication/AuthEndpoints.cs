using Dash.Domain.Common;
using Dash.Application.Features.Authentication.DTOs;
using Dash.Application.Features.Authentication.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Dash.Api.Features.Authentication;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/login", LoginAsync);
        group.MapPost("/register", RegisterAsync);

        return app;
    }

    private static async Task<Results<Ok<AuthResponse>, BadRequest<Error>>> LoginAsync(
        LoginRequest request,
        IAuthService authService)
    {
        throw new NotImplementedException();
    }

    private static async Task<Results<Ok<AuthResponse>, BadRequest<Error>>> RegisterAsync(
            LoginRequest request,
            IAuthService authService)
    {
        throw new NotImplementedException();
    }
}
