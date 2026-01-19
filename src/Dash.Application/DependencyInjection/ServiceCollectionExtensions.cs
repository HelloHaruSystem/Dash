using Dash.Application.Features.Authentication.Interfaces;
using Dash.Application.Features.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // register services ehre
        services.AddSingleton<IPasswordService, PasswordService>();

        return services;
    }
}
