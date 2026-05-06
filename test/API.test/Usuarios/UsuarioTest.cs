using Application.Auth.DTOs.Requests;
using Domain.Enums;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Usuarios;

public class UsuarioTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/Usuario";

    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        await _fixture.AuthenticateAsync(_factory, _client);
    }
    public Task DisposeAsync() => Task.CompletedTask;
    public UsuarioTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _factory = fixture.App;
    }

    [Fact]
    public async Task Usuario_Post_CriarUsuario_Administrador_Created()
    {
        // Arrange
        var request = new CriarUsuarioRequestDTO
        {
            Nome = "Novo Funcionario",
            Email = $"novo_admin_{Guid.NewGuid()}@oficina.com",
            Senha = "SenhaForte123!",
            Perfil = EPerfilUsuario.Administrador
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Usuario_Post_CriarUsuario_SemPermissao_Forbidden()
    {
        // Arrange
        using var anonymousClient = _factory.CreateClient();
        var request = new CriarUsuarioRequestDTO { Nome = "Intruso", Email = "i@i.com", Senha = "123" };

        // Act
        var result = await anonymousClient.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Usuario_Post_CriarUsuario_EmailDuplicado_BadRequest()
    {
        // Arrange
        var emailRepetido = "admin@email.com";
        var request = new CriarUsuarioRequestDTO
        {
            Nome = "Outro Admin",
            Email = emailRepetido,
            Senha = "OutraSenha123!",
            Perfil = EPerfilUsuario.Administrador
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Usuario_Post_CriarUsuario_DadosInvalidos_BadRequest()
    {
        // Arrange
        var request = new CriarUsuarioRequestDTO
        {
            Nome = "",
            Email = "email-invalido",
            Senha = ""
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

}
