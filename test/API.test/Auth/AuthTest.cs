using Application.Auth.DTOs.Requests;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Auth;

[Collection("Integration")]
public class AuthTest : IAsyncLifetime
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;

    public AuthTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _factory = fixture.App;
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Auth_Post_Login_Unauthorized()
    {
        using var client = _factory.CreateClient();

        var user = new LoginRequestDTO
        {
            Email = "tester@email.com",
            Senha = "32165465414230321"
        };

        var result = await client.PostAsJsonAsync("/api/login", user);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
}