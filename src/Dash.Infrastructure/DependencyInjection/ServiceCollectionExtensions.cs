using Dash.Application.Common.Persistence;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Infrastructure.Data;
using Dash.Infrastructure.Enums;
using Dash.Infrastructure.Options;
using Dash.Infrastructure.Persistence.Repositories;
using Dash.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        // Register and validate Jwt Options
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Configure JWT Authentication
        services.AddJwtAuthentication(configuration);

        // Get the options for immediate use
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>()
            ?? throw new InvalidOperationException("Database configuration is missing");

        // Register DbContext based on provider
        switch (databaseOptions.Provider)
        {
            case DatabaseProvider.Sqlite:
                services.AddDbContext<DashDbContext>(options =>
                    options.UseSqlite(databaseOptions.ConnectionString,
                        x => x.MigrationsHistoryTable("__ef_migrations_history")));
                break;

            case DatabaseProvider.PostgreSQL:
                services.AddDbContext<DashDbContext>(options =>
                    options.UseNpgsql(databaseOptions.ConnectionString,
                        x => x.MigrationsHistoryTable("__ef_migrations_history")));
                break;

            case DatabaseProvider.None:
            default:
                throw new InvalidOperationException(
                    $"Unsupported database provider: {databaseOptions.Provider}. " +
                    "Supported providers are: Sqlite, PostgreSQL");
        }

        // Add Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Add authentication Services
        services.AddSingleton<ITokenService, TokenService>();

        // Add Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<DashDbContext>();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration is missing");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }
}
