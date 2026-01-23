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
        Result<AuthResponse> result = await authService.LoginAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.BadRequest(result.Error);
    }

    private static async Task<Results<Ok<AuthResponse>, Conflict<Error>>> RegisterAsync(
            RegisterRequest request,
            IAuthService authService)
    {
        Result<AuthResponse> result = await authService.RegisterAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.Conflict(result.Error);
    }
}
