using Dash.Application.Common.Persistence;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Application.Features.Authentication.Services;
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
}
