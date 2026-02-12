using Dash.Application.Common.Persistence;
using Dash.Application.Features.Authentication.DTOs;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Application.Features.Authentication.Mappings;
using Dash.Domain.Common;
using Dash.Domain.Entities;
using Dash.Domain.Errors;
using Microsoft.Extensions.Logging;

namespace Dash.Application.Features.Authentication.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILoginAttemptRepository _loginAttemptRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutWindow = TimeSpan.FromMinutes(15);

    public AuthService(
            IUserRepository userRepository,
            ILoginAttemptRepository loginAttemptRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordService passwordService,
            ITokenService tokenService,
            ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _loginAttemptRepository = loginAttemptRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> LoginAsync(
            LoginRequest request,
            string? ipAddress,
            string? userAgent)
    {
        _logger.LogInformation("Login attempt for identifier: {Identifier}", request.Identifier);

        // Check if identifier matches a user in the database either by email or username
        var user = await _userRepository.GetByIdentifierAsync(request.Identifier);

        if (user is null)
        {
            _logger.LogWarning("Login failed: User not found for identifier: {Identifier}", request.Identifier);
            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        // Check if account is locked
        DateTime lockoutCutoff = DateTime.UtcNow - LockoutWindow;
        int failedAttempts = await _loginAttemptRepository.CountRecentFailedAttemptsAsync(user.Id, lockoutCutoff);
        if (failedAttempts >= MaxFailedAttempts)
        {
            _logger.LogWarning("Login failed: Account is locked for user: {Username}", user.Username);
            return Result<AuthResponse>.Failure(UserErrors.AccountIsLocked);
        }

        // Check if password matches with the user found
        // If not a match send the same InvalidCredentials Errors
        // To not give any information
        if (!await _passwordService.VerifyPasswordAsync(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: Invalid password for user: {Username}", user.Username);
            await _loginAttemptRepository.AddAsync(
                LoginAttempt.Create(user.Id, false, ipAddress, userAgent));
            await _loginAttemptRepository.SaveChangesAsync();

            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        // save successful LoginAttempt
        await _loginAttemptRepository.AddAsync(
            LoginAttempt.Create(user.Id, true, ipAddress, userAgent));

        await _loginAttemptRepository.SaveChangesAsync();

        // Generate token
        string token = _tokenService.GenerateToken(user.Id, user.Username, user.Email);

        // Generate refresh token
        RefreshToken refreshToken = _tokenService.GenerateRefreshToken(user.Id, ipAddress, userAgent);

        // Save refresh token
        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        // Map To AuthResponse
        AuthResponse response = user.ToAuthResponse(token, refreshToken.Token);

        _logger.LogInformation("User {Username} logged in successfully", user.Username);

        // Return success result
        return Result<AuthResponse>.Success(response);
    }

    public async Task<Result<AuthResponse>> RegisterAsync(
            RegisterRequest request,
            string? ipAddress,
            string? userAgent)
    {
        _logger.LogInformation("New register attempt with the email: {Email}", request.Email);

        // Check if username already exists
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
        {
            _logger.LogWarning("Registration attempt failed username already in use: {Username}", request.Username);
            return Result<AuthResponse>.Failure(UserErrors.UsernameAlreadyInUse);
        }

        // Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            _logger.LogWarning("Registration attempt failed email already in use: {Email}", request.Email);
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

        // Generate refresh token
        RefreshToken refreshToken = _tokenService.GenerateRefreshToken(newUser.Id, ipAddress, userAgent);

        // Save refresh token
        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        // Map to AuthResponse
        AuthResponse response = newUser.ToAuthResponse(token, refreshToken.Token);

        _logger.LogInformation("New user {Username} created successfully", newUser.Username);

        // Return success Result
        return Result<AuthResponse>.Success(response);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(
            RefreshRequest request,
            string? ipAddress,
            string? userAgent)
    {
        // Find the refresh token
        RefreshToken? existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        // Validate that it exists and is active
        if (existingToken is null || !existingToken.IsActive)
        {
            _logger.LogWarning("Refresh failed: Invalid or epired refresh token");
            return Result<AuthResponse>.Failure(UserErrors.InvalidRefreshToken);
        }

        // Get the user
        User? user = await _userRepository.GetByIdAsync(existingToken.UserId);

        if (user is null)
        {
            _logger.LogWarning("Refresh failed: User not found for token");
            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        // Revoke the old token
        existingToken.Revoke();

        // Generate new token
        string token = _tokenService.GenerateToken(user.Id, user.Username, user.Email);
        RefreshToken newRefreshToken = _tokenService.GenerateRefreshToken(user.Id, ipAddress, userAgent);

        // add the new token and save changes
        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        // Create response
        AuthResponse response = user.ToAuthResponse(token, newRefreshToken.Token);

        _logger.LogInformation("Tokens refreshed for User: {Username}", user.Username);

        return Result<AuthResponse>.Success(response);
    }
}
