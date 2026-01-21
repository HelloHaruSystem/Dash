namespace Dash.Application.Features.Authentication.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for an atuhenticated user
    /// </summary>
    public string GenerateToken(Guid Id, string username, string email);
}
