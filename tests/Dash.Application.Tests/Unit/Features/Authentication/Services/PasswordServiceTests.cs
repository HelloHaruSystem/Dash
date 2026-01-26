using Dash.Application.Features.Authentication.Services;

namespace Dash.Application.Tests.Unit.Features.Authentication.Services;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService;

    public PasswordServiceTests()
    {
        _passwordService = new PasswordService();
    }

    [Fact]
    public async Task HashPasswordAsync_ShouldReturnNonEmptyString()
    {
        string password = "NewTestPassword123";

        string hash = await _passwordService.HashPasswordAsync(password);

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public async Task HashPasswordAsync_SamePasswordTwice_ShouldProduceDifferentHashes()
    {
        string password = "NewTestPassword123";

        string hash1 = await _passwordService.HashPasswordAsync(password);
        string hash2 = await _passwordService.HashPasswordAsync(password);

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithCorrectPassword_ShouldReturnTrue()
    {
        string password = "NewTestPassword123";
        string wrongPassword = "WrongTestPassword123";
        string hash = await _passwordService.HashPasswordAsync(password);

        bool result = await _passwordService.VerifyPasswordAsync(wrongPassword, hash);

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("LongPassWordWithSpecialCharactersIncluded!@#$%^&*()123456")]
    public async Task HashPasswordAsync_WithVariousPasswords_ShouldSucceed(string password)
    {
        string hash = await _passwordService.HashPasswordAsync(password);

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);

        bool isValid = await _passwordService.VerifyPasswordAsync(password, hash);
        Assert.True(isValid);
    }
}
