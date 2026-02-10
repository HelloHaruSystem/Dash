using Dash.Domain.Common;
using Dash.Domain.Entities;
using Dash.Domain.Errors;
using Dash.Application.Common.Persistence;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Application.Features.Authentication.Services;
using Dash.Application.Features.Authentication.DTOs;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Dash.Application.Tests.Unit.Features.Authentication.Services;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILoginAttemptRepository _loginAttemptRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly AuthService _authService;
    private readonly ILogger<AuthService> _logger;

    public AuthServiceTests()
    {
        // create mocks
        _userRepository = Substitute.For<IUserRepository>();
        _loginAttemptRepository = Substitute.For<ILoginAttemptRepository>();
        _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        _passwordService = Substitute.For<IPasswordService>();
        _tokenService = Substitute.For<ITokenService>();
        _logger = Substitute.For<ILogger<AuthService>>();

        // Create the auth service with the mocks
        _authService = new AuthService(
                _userRepository,
                _loginAttemptRepository,
                _refreshTokenRepository,
                _passwordService,
                _tokenService,
                _logger
        );
    }

    [Fact]
    public async Task LoginAsync_CorrectLoginRequestShouldReturnSuccessResult()
    {
        User fakeUser = User.Create("testuser", "test@test.com", "fake-test-password-hash");
        string fakeToken = "fake-jwt-token";

        LoginRequest request = new()
        {
            Identifier = fakeUser.Username,
            Password = "PlainTextPassword123!"
        };

        _userRepository.GetByIdentifierAsync(Arg.Any<string>()).Returns(fakeUser);
        _passwordService.VerifyPasswordAsync("PlainTextPassword123!", fakeUser.PasswordHash).Returns(true);
        _tokenService.GenerateToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>()).Returns(fakeToken);

        _tokenService.GenerateRefreshToken(Arg.Any<Guid>(), Arg.Any<string?>(), Arg.Any<string?>())
            .Returns(RefreshToken.Create(fakeUser.Id, "fake-token", "127.0.0.1", "TestAgent", TimeSpan.FromDays(7)));

        Result<AuthResponse> result = await _authService.LoginAsync(request, "127.0.0.1", "TestAgent");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(result.Value.Username, fakeUser.Username);
        Assert.Equal(result.Value.Email, fakeUser.Email);
        Assert.Equal(result.Value.Token, fakeToken);
    }

    [Fact]
    public async Task LoginAsync_LoginRequestWithWrongIdentifierShouldReturnFailureResult()
    {
        LoginRequest request = new()
        {
            Identifier = "nonexistent@test.com",
            Password = "PlainTextPassword123!"
        };

        Result<AuthResponse> result = await _authService.LoginAsync(request, null, null);

        Assert.True(result.IsFailure);
        Assert.Null(result.Value);

        Assert.Equal(UserErrors.InvalidCredentials, result.Error);
    }

    [Fact]
    public async Task LoginAsync_LoginRequestWithWrongPasswordShouldReturnFailureResult()
    {
        User fakeUser = User.Create("testuser", "test@test.com", "fake-test-password-hash");
        LoginRequest request = new()
        {
            Identifier = fakeUser.Username,
            Password = "PlainTextPassword123!"
        };

        _userRepository.GetByIdentifierAsync(Arg.Any<string>()).Returns(fakeUser);

        Result<AuthResponse> result = await _authService.LoginAsync(request, "127.0.0.1", "TestAgent");

        Assert.True(result.IsFailure);
        Assert.Null(result.Value);

        Assert.Equal(UserErrors.InvalidCredentials, result.Error);
    }

    [Fact]
    public async Task LoginAsync_WhenAccountIsLocked_ShouldReturnAccountIsLockedError()
    {
        User fakeUser = User.Create("testuser", "test@test.com", "fake-test-password-hash");
        LoginRequest request = new()
        {
            Identifier = fakeUser.Username,
            Password = "PlainTextPassword123!"
        };

        _userRepository.GetByIdentifierAsync(Arg.Any<string>()).Returns(fakeUser);
        _loginAttemptRepository.CountRecentFailedAttemptsAsync(fakeUser.Id, Arg.Any<DateTime>()).Returns(5);

        Result<AuthResponse> result = await _authService.LoginAsync(request, "127.0.0.1", "TestAgent");

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AccountIsLocked, result.Error);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsWrong_ShouldRecordFailedAttempt()
    {
        User fakeUser = User.Create("testuser", "test@test.com", "fake-test-password-hash");
        LoginRequest request = new()
        {
            Identifier = fakeUser.Username,
            Password = "WrongPassword123!"
        };

        _userRepository.GetByIdentifierAsync(Arg.Any<string>()).Returns(fakeUser);
        _loginAttemptRepository.CountRecentFailedAttemptsAsync(fakeUser.Id, Arg.Any<DateTime>()).Returns(0);
        _passwordService.VerifyPasswordAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        await _authService.LoginAsync(request, "127.0.0.1", "TestAgent");

        await _loginAttemptRepository.Received(1).AddAsync(Arg.Is<LoginAttempt>(a =>
            a.UserId == fakeUser.Id &&
            !a.IsSuccessful));
        await _loginAttemptRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task LoginAsync_WhenLoginSucceeds_ShouldRecordSuccessfulAttempt()
    {
        User fakeUser = User.Create("testuser", "test@test.com", "fake-test-password-hash");
        LoginRequest request = new()
        {
            Identifier = fakeUser.Username,
            Password = "CorrectPassword123!"
        };

        _userRepository.GetByIdentifierAsync(Arg.Any<string>()).Returns(fakeUser);
        _loginAttemptRepository.CountRecentFailedAttemptsAsync(fakeUser.Id, Arg.Any<DateTime>()).Returns(0);
        _passwordService.VerifyPasswordAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        _tokenService.GenerateToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>()).Returns("fake-token");

        _tokenService.GenerateRefreshToken(Arg.Any<Guid>(), Arg.Any<string?>(), Arg.Any<string?>())
            .Returns(RefreshToken.Create(fakeUser.Id, "fake-token", "127.0.0.1", "TestAgent", TimeSpan.FromDays(7)));

        await _authService.LoginAsync(request, "127.0.0.1", "TestAgent");

        await _loginAttemptRepository.Received(1).AddAsync(Arg.Is<LoginAttempt>(a =>
            a.UserId == fakeUser.Id &&
            a.IsSuccessful));
        await _loginAttemptRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task RegisterAsync_CorrectRegisterRequestShouldReturnSuccessResult()
    {
        RegisterRequest request = new()
        {
            Username = "fake-user-name",
            Email = "fakemail@mail.com",
            Password = "fake-plain-text-passwprd"
        };

        _userRepository.ExistsByUsernameAsync(Arg.Any<string>()).Returns(false);
        _userRepository.ExistsByEmailAsync(Arg.Any<string>()).Returns(false);
        _passwordService.HashPasswordAsync(Arg.Any<string>()).Returns("fake-hashed-password");
        _tokenService.GenerateToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>()).Returns("fake-token");

        _tokenService.GenerateRefreshToken(Arg.Any<Guid>(), Arg.Any<string?>(), Arg.Any<string?>())
            .Returns(RefreshToken.Create(Guid.NewGuid(), "fake-token", "127.0.0.1", "TestAgent", TimeSpan.FromDays(7)));

        Result<AuthResponse> result = await _authService.RegisterAsync(request, "127.0.0.1", "TestAgent");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(request.Username, result.Value.Username);
        Assert.Equal(request.Email, result.Value.Email);
        Assert.Equal("fake-token", result.Value.Token);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ShouldReturnUsernameAlreadyInUse()
    {
        RegisterRequest request = new()
        {
            Username = "fake-user-name",
            Email = "fakemail@mail.com",
            Password = "fake-plain-text-passwprd"
        };

        _userRepository.ExistsByUsernameAsync(Arg.Any<string>()).Returns(true);

        Result<AuthResponse> result = await _authService.RegisterAsync(request, "127.0.0.1", "TestAgent");

        Assert.True(result.IsFailure);
        Assert.Null(result.Value);

        Assert.Equal(UserErrors.UsernameAlreadyInUse, result.Error);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnEmailAlreadyInUse()
    {
        RegisterRequest request = new()
        {
            Username = "fake-user-name",
            Email = "fakemail@mail.com",
            Password = "fake-plain-text-passwprd"
        };

        _userRepository.ExistsByUsernameAsync(Arg.Any<string>()).Returns(false);
        _userRepository.ExistsByEmailAsync(Arg.Any<string>()).Returns(true);

        Result<AuthResponse> result = await _authService.RegisterAsync(request, "127.0.0.1", "TestAgent");

        Assert.True(result.IsFailure);
        Assert.Null(result.Value);

        Assert.Equal(UserErrors.EmailAlreadyInUse, result.Error);
    }
}
