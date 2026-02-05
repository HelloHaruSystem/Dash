using Dash.Domain.Common;
using Dash.Api.Common;
using Dash.Application.Features.Authentication.DTOs;
using Dash.Application.Features.Authentication.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Dash.Application.Common.Persistence;
using System.Security.Claims;
using Dash.Domain.Entities;

namespace Dash.Api.Features.Authentication;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/login", LoginAsync)
            .AddEndpointFilter<ValidationFilter<LoginRequest>>()
            .RequireRateLimiting(RateLimitPolicies.Login);

        group.MapPost("/register", RegisterAsync)
            .AddEndpointFilter<ValidationFilter<RegisterRequest>>()
            .RequireRateLimiting(RateLimitPolicies.Register);

        group.MapGet("/test-me", GetCurrentUserAsync)
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitPolicies.Api);

        return app;
    }

    private static async Task<Results<Ok<AuthResponse>, JsonHttpResult<Error>>> LoginAsync(
        LoginRequest request,
        IAuthService authService,
        HttpContext context)
    {
        string? ipAddress = context.Connection.RemoteIpAddress?.ToString();
        string? userAgent = context.Request.Headers.UserAgent.ToString();

        Result<AuthResponse> result = await authService.LoginAsync(request, ipAddress, userAgent);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.Json(result.Error, statusCode: ErrorMapper.ToStatusCode(result.Error));
    }

    private static async Task<Results<Created<AuthResponse>, Conflict<Error>>> RegisterAsync(
            RegisterRequest request,
            IAuthService authService)
    {
        Result<AuthResponse> result = await authService.RegisterAsync(request);

        return result.IsSuccess
            ? TypedResults.Created(uri: string.Empty, value: result.Value)
            : TypedResults.Conflict(result.Error);
    }

    private static async Task<Results<Ok<AuthResponse>, NotFound>> GetCurrentUserAsync(
            HttpContext context,
            IUserRepository userRepository)
    {
        // Extract user Id From JWT
        string? userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return TypedResults.NotFound();
        }

        // Fetch user from database
        User? user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return TypedResults.NotFound();
        }

        // return user data
        return TypedResults.Ok(new AuthResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Token = string.Empty // no new token
        });
    }
}
