using Dash.Application.Common.Persistence;
using Dash.Application.Features.Authentication.DTOs;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Application.Features.Authentication.Mappings;
using Dash.Domain.Common;
using Dash.Domain.Entities;
using Dash.Domain.Errors;

namespace Dash.Application.Features.Authentication.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IPasswordService passwordService, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        // Check if identifier matches a user in the database either by email or username
        var user = await _userRepository.GetByIdentifierAsync(request.Identifier);

        if (user is null)
        {
            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        // Check if password matches with the user found
        // If not a match send the same InvalidCredentials Errors
        // To not give any information
        if (!await _passwordService.VerifyPasswordAsync(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        // Generate token
        string token = _tokenService.GenerateToken(user.Id, user.Username, user.Email);

        // Map To AuthResponse
        AuthResponse response = user.ToAuthResponse(token);

        // Return success result
        return Result<AuthResponse>.Success(response);
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        // Check if username already exists
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
        {
            return Result<AuthResponse>.Failure(UserErrors.UsernameAlreadyInUse);
        }

        // Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            return Result<AuthResponse>.Failure(UserErrors.EmailAlreadyInUse);
        }

        // Hash the password
        string passwordHash = await _passwordService.HashPasswordAsync(request.Password);

        // Create User entity
        User newUser = User.Create(request.Username, request.Email, passwordHash);

        // Add user to repository
        await _userRepository.AddAsync(newUser);

        // Save changes
        await _userRepository.SaveChangesAsync();

        // Generate token
        string token = _tokenService.GenerateToken(newUser.Id, newUser.Username, newUser.Email);

        // Map to AuthResponse
        AuthResponse response = newUser.ToAuthResponse(token);

        // Return success Result
        return Result<AuthResponse>.Success(response);
    }
}
