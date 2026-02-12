namespace Dash.Application.Features.Authentication.Interfaces;

public interface IPasswordService
{
    Task<string> HashPasswordAsync(string password);
    Task<bool> VerifyPasswordAsync(string password, string passwordHash);
}
