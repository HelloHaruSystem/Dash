using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Dash.Api.Extensions;

internal static class RateLimitingExtensions
{
    /// <summary>
    /// Adds the default API rate limiting policy
    /// </summary>
    internal static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Default policy for general API endpoints
            // 100 request per minute per IP address
            options.AddFixedWindowLimiter("api", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0; // no queuing, reject
            });
        });

        return services;
    }
}
