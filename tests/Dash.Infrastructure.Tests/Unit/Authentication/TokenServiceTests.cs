using Dash.Infrastructure.Authentication;
using Dash.Infrastructure.Options;
using MsOptions = Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Dash.Infrastructure.Tests.Unit.Authentication;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        // Create fake JwtOptions
        JwtOptions jwtOptions = new JwtOptions
        {
            Secret = "test-secret-key-that-is-at-least-32-characters-long!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiresInMinutes = 60
        };

        // Wrap it in IOptions
        var options = MsOptions.Options.Create(jwtOptions);

        _tokenService = new TokenService(options);
    }

    [Fact]
    public void GenerateToken_ShouldReturnNonEmptyString()
    {
        Guid id = Guid.NewGuid();
        string username = "testUsername";
        string email = "test@test.com";

        string token = _tokenService.GenerateToken(id, username, email);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateToken_TokensShouldBeUnique()
    {
        Guid id = Guid.NewGuid();
        string username = "testUsername";
        string email = "test@test.com";

        string token1 = _tokenService.GenerateToken(id, username, email);
        string token2 = _tokenService.GenerateToken(id, username, email);

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateToken_ShouldProduceValidJwtToken()
    {
        // For a valid Jwt token the test should be able to decode the token and read its claims
        JwtSecurityTokenHandler jwtTokenHandler = new();
        Guid id = Guid.NewGuid();
        string username = "testUsername";
        string email = "test@test.com";

        string token = _tokenService.GenerateToken(id, username, email);
        JwtSecurityToken decodedJwtToken = jwtTokenHandler.ReadJwtToken(token);

        // Get specific claims
        Claim? subClaim = decodedJwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        Claim? userNameClaim = decodedJwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
        Claim? emailClaim = decodedJwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);

        // Assert they exists and have the correct values
        Assert.NotNull(subClaim);
        Assert.Equal(id.ToString(), subClaim.Value);

        Assert.NotNull(userNameClaim);
        Assert.Equal(username, userNameClaim.Value);

        Assert.NotNull(emailClaim);
        Assert.Equal(email, emailClaim.Value);
    }
}
