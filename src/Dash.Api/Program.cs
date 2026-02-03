using Dash.Api.Features.Authentication;
using Dash.Api.Extensions;
using Dash.Api.Common;
using Dash.Infrastructure.DependencyInjection;
using Dash.Application.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add Infrastructure (DbContext, Options, etc)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application (like services)
builder.Services.AddApplication();

// Add Rate Limiting
builder.Services.AddApiRateLimiting();

// Add problem details for error responses
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

// Exception handler important to be first in middleware pipeline
// THis cathces all unhandled exceptions and returns responses
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // use HSTS in production
    app.UseHsts();
}

app.UseHttpsRedirection();

// app.UseRouting(); // built in with Minimal API
app.UseRateLimiter();
// app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

// Endpoints
// Root
app.MapGet("/", () => TypedResults.Ok("Hello, World!"))
    .RequireRateLimiting(RateLimitPolicies.Api);
// Health Check
app.MapHealthChecks("/Health");

app.Run();

// Make this class public instead of internal
// So it is testable
public partial class Program { }
