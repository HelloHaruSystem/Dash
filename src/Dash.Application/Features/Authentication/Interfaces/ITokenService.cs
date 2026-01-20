namespace Dash.Application.Features.Authentication.Interfaces;

public interface ITokenService
{
    /// <sumamry>
    /// Generates a JWT token for an atuhenticated user
    /// </summary>
    public string GenerateToken(Guid Id, string email, string username);
}
