using Dash.Application.Interfaces;
using Dash.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // register services ehre
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}
