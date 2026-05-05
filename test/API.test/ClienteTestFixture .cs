using API.test;

public class ClientTestFixture : IAsyncLifetime
{
    public HttpClient Client { get; private set; } = default!;
    public ApiWebApplicationFactory App { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        App = new ApiWebApplicationFactory();
        Client = App.CreateClient();

        var integrationTestBase = new IntegrationTestBase();
        var token = await integrationTestBase.AuthenticateAsync();

        Client.DefaultRequestHeaders.Authorization = token;
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }
}