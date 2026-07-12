namespace API.test;

public class IntegrationTestFixture : IntegrationTestBase, IAsyncLifetime
{
    public ApiWebApplicationFactory App { get; private set; } = new();
    public HttpClient Client { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await App.InitializeAsync();

        Client = App.CreateClient();
        await App.ResetDatabaseAsync();
        Client.DefaultRequestHeaders.Authorization = await AuthenticateAsync(App, Client);
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();

        await ((IAsyncLifetime)App).DisposeAsync();
    }
}