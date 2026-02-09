using Dash.Domain.Entities;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Infrastructure.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Dash.Infrastructure.Authentication;

public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtOptions> options, ILogger<TokenService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string GenerateToken(Guid Id, string username, string email)
    {
        // Create Claims (user data to embed into the token)
        Claim[] claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Create signing credentials using your secret key
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create Tokens
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiresInMinutes),
            signingCredentials: credentials
        );

        _logger.LogInformation("Generated JWT Token for User: {Username}, Expires: {ExpireaAt}", username, token.ValidTo);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(Guid userId, string? ipAddress, string? deviceInfo)
    {
        TimeSpan lifetime = TimeSpan.FromDays(_options.RefreshTokenExpiresInDays);
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        RefreshToken refreshToken = RefreshToken.Create(userId, token, ipAddress, deviceInfo, lifetime);

        _logger.LogInformation("Generated Refresh Token for UserId: {UserId}, Expires: {ExpiresAt}",
                    userId, refreshToken.ExpiresAt);

        return refreshToken;
    }
}
