
namespace Dash.Api.Extensions;

internal static class CorsExtensions
{
    /// <summary>
    /// Cors Settings
    /// </summary>
    internal static IServiceCollection AddApiCors(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        // Default CORS policy for allowedOrigins
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });
        });

        // Add more policies here

        return services;
    }
}
