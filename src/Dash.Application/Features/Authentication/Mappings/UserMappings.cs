using Dash.Application.Features.Authentication.DTOs;
using Dash.Domain.Entities;

namespace Dash.Application.Features.Authentication.Mappings;

internal static class UserMappings
{
    /// <summary>
    /// Maps a RegisterRquest DTO to a user entity
    /// </summary>
    public static User ToEntity(this RegisterRequest request, string passwordHash)
    {
        return User.Create(
            request.Username,
            request.Email,
            passwordHash
        );
    }

    /// <summary>
    /// Maps a User entity to an AuthResponse DTO
    /// </summary>
    public static AuthResponse ToAuthResponse(this User user, string token)
    {
        return new AuthResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Token = token
        };
    }
}
