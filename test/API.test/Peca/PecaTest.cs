using Application.Pecas.DTOs.Requests;
using Domain.Aggregates.EstoqueAggregates;
using Infra.Context;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Domain.Entities;

namespace API.test.Peca;

public class PecaTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/Peca";
    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;
    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    public PecaTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _factory = fixture.App;
    }

    [Fact]
    public async Task Peca_Post_Create_Created()
    {
        // Arrange 
        var request = new PecaRequestDTO
        {
            Nome = "Amortecedor Dianteiro",
            Descricao = "Cofap Turbo Gás",
            MarcaPeca = "Cofap",
            PrecoVenda = 350.00m
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Get_GetById_OK()
    {
        // Arrange
        Guid pecaId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var peca = new Domain.Entities.Peca(
                "Disco de Freio",
                "Ventilado",
                "Brembo",
                180.00m,
                Guid.Empty,
                DateTime.UtcNow
            );

            context.Pecas.Add(peca);
            await context.SaveChangesAsync();
            pecaId = peca.Id;
        }

        // Act
        var result = await _client.GetAsync($"{ApiKey}/{pecaId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Put_Update_NoContent()
    {
        // Arrange
        Guid pecaId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var peca = new Domain.Entities.Peca("Vela", "Iridium", "NGK", 60.00m, Guid.Empty, DateTime.UtcNow);
            context.Pecas.Add(peca);
            await context.SaveChangesAsync();
            pecaId = peca.Id;
        }

        var updateRequest = new PecaRequestDTO
        {
            Nome = "Vela Alterada",
            Descricao = "Iridium Premium",
            MarcaPeca = "NGK",
            PrecoVenda = 85.00m
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{pecaId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Delete_Delete_NoContent()
    {
        // Arrange
        Guid pecaId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var peca = new Domain.Entities.Peca("Filtro", "Ar", "Fram", 45.00m, Guid.Empty, DateTime.UtcNow);
            context.Pecas.Add(peca);

            context.Estoques.Add(new Domain.Aggregates.EstoqueAggregates.Estoque(null, peca.Id ,0, Guid.Empty, DateTime.UtcNow));

            await context.SaveChangesAsync();
            pecaId = peca.Id;
        }

        // Act
        var result = await _client.DeleteAsync($"{ApiKey}/{pecaId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Post_Create_InvalidData_BadRequest()
    {
        // Arrange: Nome vazio e preço negativo (campos que devem falhar na validação)
        var request = new PecaRequestDTO
        {
            Nome = "",
            Descricao = "Teste de erro",
            MarcaPeca = "Marca",
            PrecoVenda = -10.00m
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Get_GetById_NonExistent_NotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var result = await _client.GetAsync($"{ApiKey}/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Delete_WithStockPositive_BadRequest()
    {
        // Arrange
        Guid pecaId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var peca = new Domain.Entities.Peca("Peca Travada", "Desc", "Marca", 50, Guid.Empty, DateTime.UtcNow);
            context.Pecas.Add(peca);

            context.Estoques.Add(new Domain.Aggregates.EstoqueAggregates.Estoque(null, peca.Id, 10, Guid.Empty, DateTime.UtcNow));

            await context.SaveChangesAsync();
            pecaId = peca.Id;
        }

        // Act
        var result = await _client.DeleteAsync($"{ApiKey}/{pecaId}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Put_Update_NonExistent_NotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();
        var updateRequest = new PecaRequestDTO
        {
            Nome = "Novo Nome",
            Descricao = "Desc",
            MarcaPeca = "Marca",
            PrecoVenda = 100
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{idInexistente}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}
