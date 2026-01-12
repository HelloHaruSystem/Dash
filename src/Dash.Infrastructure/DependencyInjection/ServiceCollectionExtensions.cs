using Dash.Infrastructure.Data;
using Dash.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dash.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        // Register and validate DatabaseOptions
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Get the options for immediate use
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>()
            ?? throw new InvalidOperationException("Database configuration is missing");

        // Register DbContext based on provider
        switch (databaseOptions.Provider)
        {
            case "Sqlite":
                services.AddDbContext<DashDbContext>(options =>
                    options.UseSqlite(databaseOptions.ConnectionString));
                break;

            case "PostgreSQL":
                services.AddDbContext<DashDbContext>(options =>
                    options.UseNpgsql(databaseOptions.ConnectionString));
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported database provider: {databaseOptions.Provider}. " +
                    "Supported providers are: Sqlite, PostgreSQL");
        }

        return services;
    }
}
