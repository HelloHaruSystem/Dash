using Dash.Domain.Common;
using Dash.Domain.Entities;
using Dash.Domain.Errors;
using Dash.Application.Common.Persistence;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Application.Features.Authentication.Services;
using Dash.Application.Features.Authentication.DTOs;
using NSubstitute;

namespace Dash.Application.Tests.Features.Authentication.Services;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // create mocks
        _userRepository = Substitute.For<IUserRepository>();
        _passwordService = Substitute.For<IPasswordService>();
        _tokenService = Substitute.For<ITokenService>();

        // Create the auth service with the mocks
        _authService = new AuthService(_userRepository, _passwordService, _tokenService);
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

        Result<AuthResponse> result = await _authService.LoginAsync(request);

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

        Result<AuthResponse> result = await _authService.LoginAsync(request);

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

        Result<AuthResponse> result = await _authService.LoginAsync(request);

        Assert.True(result.IsFailure);
        Assert.Null(result.Value);

        Assert.Equal(UserErrors.InvalidCredentials, result.Error);
    }
}
