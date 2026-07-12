using Infra.Context;
using Infra.DbModel;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTOs.Servicos.Requests;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Servicos;

[Collection("Integration")]
public class ServicoTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/servicos";

    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _client.DefaultRequestHeaders.Authorization = await _fixture.AuthenticateAsync(_factory, _client);
    }
    public Task DisposeAsync() => Task.CompletedTask;

    public ServicoTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _factory = fixture.App;
    }
    [Fact]
    public async Task Servico_Post_Create_Created()
    {
        // Arrange
        var request = new ServicoRequestDTO
        {
            Nome = "Troca de Óleo",
            Descricao = "Troca de óleo sintético e filtro",
            PrecoVenda = 150.00m
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Get_GetPaginated_PartialContent()
    {
        // Act
        var result = await _client.GetAsync(ApiKey);

        // Assert
        Assert.True(result.StatusCode == HttpStatusCode.OK ||
                    result.StatusCode == HttpStatusCode.PartialContent);
    }

    [Fact]
    public async Task Servico_Get_GetById_OK()
    {
        // Arrange
        Guid servicoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var admin = context.Usuarios.First();
            var servico = new ServicoDbModel(
                Guid.NewGuid(), "Alinhamento", "Alinhamento e balanceamento 3D", 120.00m,
                null, admin.Id, DateTime.UtcNow, null, null, true
            );

            context.Servicos.Add(servico);
            await context.SaveChangesAsync();
            servicoId = servico.Id;
        }

        // Act
        var result = await _client.GetAsync($"{ApiKey}/{servicoId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Put_Update_NoContent()
    {
        // Arrange
        Guid servicoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var admin = context.Usuarios.First();
            var servico = new ServicoDbModel(
                Guid.NewGuid(), "Lavagem", "Lavagem simples", 50.00m,
                null, admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Servicos.Add(servico);
            await context.SaveChangesAsync();
            servicoId = servico.Id;
        }

        var updateRequest = new ServicoRequestDTO
        {
            Nome = "Lavagem Completa",
            Descricao = "Lavagem com cera e aspiração",
            PrecoVenda = 80.00m
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{servicoId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Delete_Delete_NoContent()
    {
        // Arrange
        Guid servicoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var admin = context.Usuarios.First();
            var servico = new ServicoDbModel(
                Guid.NewGuid(), "Revisão", "Revisão Geral", 200.00m,
                null, admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Servicos.Add(servico);
            await context.SaveChangesAsync();
            servicoId = servico.Id;
        }

        // Act
        var result = await _client.DeleteAsync($"{ApiKey}/{servicoId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Post_Create_InvalidData_BadRequest()
    {
        // Arrange
        var request = new ServicoRequestDTO
        {
            Nome = "",
            Descricao = "Teste de falha",
            PrecoVenda = -50.00m
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Get_GetById_NonExistent_NotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var result = await _client.GetAsync($"{ApiKey}/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Put_Update_NonExistent_NotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();
        var updateRequest = new ServicoRequestDTO
        {
            Nome = "Nome Fantasma",
            Descricao = "Desc",
            PrecoVenda = 100
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{idInexistente}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Post_Create_Anonymous_Unauthorized()
    {
        // Arrange
        using var anonymousClient = _factory.CreateClient();
        var request = new ServicoRequestDTO { Nome = "S", Descricao = "D", PrecoVenda = 10 };

        // Act
        var result = await anonymousClient.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Get_GetPaginated_PaginaZero_BadRequest()
    {
        // Act 
        var result = await _client.GetAsync($"{ApiKey}?page=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Put_Update_PrecoNegativo_BadRequest()
    {
        // Arrange
        Guid servicoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var servico = new ServicoDbModel(
                Guid.NewGuid(), "Teste", "Desc", 10m,
                null, admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Servicos.Add(servico);
            await context.SaveChangesAsync();
            servicoId = servico.Id;
        }

        var updateRequest = new ServicoRequestDTO
        {
            Nome = "Teste Atualizado",
            Descricao = "Desc",
            PrecoVenda = -1.0m // Valor inválido
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{servicoId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Servico_Delete_NonExistent_NotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var result = await _client.DeleteAsync($"{ApiKey}/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}
