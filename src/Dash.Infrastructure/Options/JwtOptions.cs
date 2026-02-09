namespace Dash.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Secret { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required int ExpiresInMinutes { get; set; }
    public required int RefreshTokenExpiresInDays { get; set; }
}
