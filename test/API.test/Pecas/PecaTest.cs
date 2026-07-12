using Infra.Context;
using Infra.DbModel;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTOs.Pecas.Request;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Pecas;

[Collection("Integration")]
public class PecaTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/pecas";
    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _client.DefaultRequestHeaders.Authorization = await _fixture.AuthenticateAsync(_factory, _client);
    }
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

            var admin = context.Usuarios.First();
            var peca = new PecaDbModel(
                Guid.NewGuid(), "Disco de Freio", "Ventilado", "Brembo", 180.00m,
                admin.Id, DateTime.UtcNow, null, null, true
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

            var admin = context.Usuarios.First();
            var peca = new PecaDbModel(
                Guid.NewGuid(), "Vela", "Iridium", "NGK", 60.00m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
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

            var admin = context.Usuarios.First();
            var peca = new PecaDbModel(
                Guid.NewGuid(), "Filtro", "Ar", "Fram", 45.00m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Pecas.Add(peca);

            context.Estoques.Add(new EstoqueDbModel(
                Guid.NewGuid(), peca.Id, null, 0, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            ));

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

            var admin = context.Usuarios.First();
            var peca = new PecaDbModel(
                Guid.NewGuid(), "Peca Travada", "Desc", "Marca", 50m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Pecas.Add(peca);

            context.Estoques.Add(new EstoqueDbModel(
                Guid.NewGuid(), peca.Id, null, 10, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            ));

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

    [Fact]
    public async Task Peca_Get_GetPaginated_OK()
    {
        // Act
        var result = await _client.GetAsync($"{ApiKey}?page=1&pageSize=5");

        // Assert 
        Assert.Equal(HttpStatusCode.PartialContent, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Get_GetPaginated_PaginaInvalida_BadRequest()
    {
        // Act 
        var result = await _client.GetAsync($"{ApiKey}?page=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Get_GetPaginated_Unauthorized()
    {
        // Arrange 
        using var anonymousClient = _factory.CreateClient();

        // Act
        var result = await anonymousClient.GetAsync(ApiKey);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Peca_Put_Update_DadosInvalidos_BadRequest()
    {
        // Arrange
        Guid pecaId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var peca = new PecaDbModel(
                Guid.NewGuid(), "Original", "Desc", "Marca", 50m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Pecas.Add(peca);
            await context.SaveChangesAsync();
            pecaId = peca.Id;
        }

        var updateRequest = new PecaRequestDTO
        {
            Nome = "", 
            PrecoVenda = -1.0m
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{pecaId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}
