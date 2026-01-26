using System.Net;
using System.Net.Http.Json;
using Dash.Application.Features.Authentication.DTOs;
using Dash.Application.Features.Authentication.Interfaces;
using Dash.Domain.Common;
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
}
