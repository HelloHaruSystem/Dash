using Dash.Application.Features.Authentication.Interfaces;
using Dash.Application.Features.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // register services here
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
