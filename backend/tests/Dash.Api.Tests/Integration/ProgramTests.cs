using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Dash.Api.Tests.Integration;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Root_Should_returnHelloWorld()
    {
        HttpResponseMessage? response = await _client.GetAsync("/");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Hello, World!", content);
    }
}
