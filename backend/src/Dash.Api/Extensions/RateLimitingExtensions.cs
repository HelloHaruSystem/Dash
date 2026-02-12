using Dash.Api.Common;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Dash.Api.Extensions;

internal static class RateLimitingExtensions
{
    /// <summary>
    /// Rate limiting policies for API endpoints
    /// </summary>
    internal static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Default policy for general API endpoints
            // 100 request per minute per IP address
            options.AddFixedWindowLimiter(RateLimitPolicies.Api, limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0; // no queuing, reject
            });


            // Login policy. More strict to avoid brute force
            // 10 request per minute per IP address
            options.AddFixedWindowLimiter(RateLimitPolicies.Login, limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0; // no queuing, reject
            });

            // Register policy. More strict to avoid spam fake accounts
            // 5 request per minute per IP address
            options.AddFixedWindowLimiter(RateLimitPolicies.Register, limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0; // no queuing, reject
            });
        });

        return services;
    }
}
