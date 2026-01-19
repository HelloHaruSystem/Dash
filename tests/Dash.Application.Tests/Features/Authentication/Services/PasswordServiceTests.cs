using Dash.Application.Features.Authentication.Services;

namespace Dash.Application.Tests.Features.Authentication.Services;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService;

    public PasswordServiceTests()
    {
        _passwordService = new PasswordService();
    }

    [Fact]
    public void HashPassword_ShouldReturnNonEmptyString()
    {
        string password = "NewTestPassword123";

        string hash = _passwordService.HashPassword(password);

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashPassword_SamePasswordTwice_ShouldProduceDifferentHashes()
    {
        string password = "NewTestPassword123";

        string hash1 = _passwordService.HashPassword(password);
        string hash2 = _passwordService.HashPassword(password);

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        string password = "NewTestPassword123";
        string wrongPassword = "WrongTestPassword123";
        string hash = _passwordService.HashPassword(password);

        bool result = _passwordService.VerifyPassword(wrongPassword, hash);

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("LongPassWordWithSpecialCharactersIncluded!@#$%^&*()123456")]
    public void HashPassword_WithVariousPasswords_ShouldSucceed(string password)
    {
        string hash = _passwordService.HashPassword(password);

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);

        bool isValid = _passwordService.VerifyPassword(password, hash);
        Assert.True(isValid);
    }
}
