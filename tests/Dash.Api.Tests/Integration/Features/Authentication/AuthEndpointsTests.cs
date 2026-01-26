using System.Net;
using System.Net.Http.Json;
using Dash.Application.Features.Authentication.DTOs;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Domain.Common;
using Dash.Domain.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;

namespace Dash.Api.Tests.Integration.Features.Authentication;

public class AuthEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Create mock services
        IAuthService authServiceMock = Substitute.For<IAuthService>();
        LoginRequest request = new LoginRequest { Identifier = "testuser", Password = "password" };

        AuthResponse expectedResponse = new AuthResponse
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            Token = "mock-jwt-token"
        };

        authServiceMock.LoginAsync(Arg.Any<LoginRequest>())
            .Returns(Result<AuthResponse>.Success(expectedResponse));

        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => authServiceMock);
            });
        }).CreateClient();

        HttpResponseMessage? response = await client.PostAsJsonAsync("/api/auth/login", request);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AuthResponse? result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Token, result.Token);
    }

    [Fact]
    public async Task Login_ShoulReturn_CorrectError_WhenCredentialsAreInvalid()
    {
        IAuthService authServiceMock = Substitute.For<IAuthService>();
        Error expectedError = UserErrors.InvalidCredentials;

        authServiceMock.LoginAsync(Arg.Any<LoginRequest>())
            .Returns(Result<AuthResponse>.Failure(expectedError));

        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => authServiceMock);
            });
        }).CreateClient();

        HttpResponseMessage? response = await client.PostAsJsonAsync(
                "/api/auth/login", new LoginRequest { Identifier = "user", Password = "123" });

        Assert.NotNull(response);
        Error? error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.NotNull(error);
        Assert.Equal(expectedError.Code, error.Code);
        Assert.Equal(expectedError.Description, error.Description);
    }

    [Theory]
    [InlineData("", "password123")] // Empty Identifier
    [InlineData("   ", "password123")] // whitespace identifier
    [InlineData("long_id", "")] // empty password
    [InlineData("too_long_id", "password123")]
    public async Task Login_ShouldReturnBadRequest_WehnDataAnnotationsFail(string identifier, string password)
    {
        IAuthService authServiceMock = Substitute.For<IAuthService>();

        string testIdentifier = identifier == "too_long_id"
            ? new string('a', 256)
            : identifier;

        LoginRequest request = new LoginRequest
        {
            Identifier = testIdentifier,
            Password = password
        };

        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => authServiceMock);
            });
        }).CreateClient();

        HttpResponseMessage? response = await client.PostAsJsonAsync("/api/auth/login", request);

        Assert.NotNull(response);

        // Correct status status code
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Check that the rrequest was stopped by the fitler
        // so that it never touched the service logic
        await authServiceMock.DidNotReceive().LoginAsync(Arg.Any<LoginRequest>());

        // Check that the response is a validation problem (error dictionary)
        HttpValidationProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.NotEmpty(problemDetails.Errors);
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenDataIsValid()
    {
        IAuthService authServiceMock = Substitute.For<IAuthService>();
        RegisterRequest request = new RegisterRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "newsecurepassword"
        };
        AuthResponse expectedResponse = new AuthResponse
        {
            Id = Guid.NewGuid(),
            Username = "newuser",
            Email = "newuser@example.com",
            Token = "newtoken"
        };

        authServiceMock.RegisterAsync(Arg.Any<RegisterRequest>())
            .Returns(Result<AuthResponse>.Success(expectedResponse));


        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => authServiceMock);
            });
        }).CreateClient();

        HttpResponseMessage? response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenUsernameAlreadyInUse()
    {
        IAuthService authServiceMock = Substitute.For<IAuthService>();
        RegisterRequest request = new RegisterRequest
        {
            Username = "existinguser",
            Email = "test@test.com",
            Password = "Password123!"
        };
        Error expectedError = UserErrors.UsernameAlreadyInUse;

        authServiceMock.RegisterAsync(Arg.Any<RegisterRequest>())
            .Returns(Result<AuthResponse>.Failure(expectedError));

        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => authServiceMock);
            });
        }).CreateClient();

        HttpResponseMessage? response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Error? error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.NotNull(error);
        Assert.Equal(expectedError.Code, error?.Code);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyInUse()
    {
        IAuthService authServiceMock = Substitute.For<IAuthService>();
        RegisterRequest request = new RegisterRequest
        {
            Username = "newuser",
            Email = "used@test.com",
            Password = "Password123!"
        };
        Error expectedError = UserErrors.EmailAlreadyInUse;

        authServiceMock.RegisterAsync(Arg.Any<RegisterRequest>())
            .Returns(Result<AuthResponse>.Failure(expectedError));

        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => authServiceMock);
            });
        }).CreateClient();

        HttpResponseMessage? response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Error? error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.NotNull(error);
        Assert.Equal(expectedError.Code, error?.Code);
    }

    [Theory]
    [InlineData("", "test@test.com", "Password123!")] // empty username
    [InlineData("user", "not-an-email", "Password123!")] // Invalid Email format
    [InlineData("user", "test@test.com", "")]            // empty password
    public async Task Register_ShouldReturnBadRequest_WhenValidationFails(string username, string email, string password)
    {
        IAuthService authServiceMock = Substitute.For<IAuthService>();
        RegisterRequest request = new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = password
        };

        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => authServiceMock);
            });
        }).CreateClient();

        HttpResponseMessage? response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Check that the fitler interrupted the request
        await authServiceMock.DidNotReceive().RegisterAsync(Arg.Any<RegisterRequest>());
    }
}
