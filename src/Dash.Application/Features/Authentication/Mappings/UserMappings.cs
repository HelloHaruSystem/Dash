using Dash.Application.Features.Authentication.DTOs;
using Dash.Domain.Entities;

namespace Dash.Application.Features.Authentication.Mappings;

public static class UserMappings
{
    public static User ToEntity(this RegisterRequest request, string passwordHash)
    {
        return new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            EmailVerified = false,
            IsActive = true,
            CreatedAt = DateTime.Now,
            PreferredLanguage = "en",
            FailedLoginAttempts = 0
        };
    }
}
