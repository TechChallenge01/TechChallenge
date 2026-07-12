using Infra.Context;
using Infra.DbModel;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTOs.Insumos.Request;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Insumos;

public class InsumoTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/insumos";
    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _client.DefaultRequestHeaders.Authorization = await _fixture.AuthenticateAsync(_factory, _client);
    }
    public Task DisposeAsync() => Task.CompletedTask;

    public InsumoTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _factory = fixture.App;
    }

    [Fact]
    public async Task Insumo_Get_GetPaginated_OK()
    {
        // act
        var result = await _client.GetAsync(ApiKey);

        // assert
        Assert.Equal(HttpStatusCode.PartialContent, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Get_GetPaginated_Unauthorized()
    {
        // arrange 
        using var anonymousClient = _factory.CreateClient();

        // act
        var result = await anonymousClient.GetAsync(ApiKey);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Post_Create_Created()
    {
        // arrange
        var request = new InsumoRequestDTO
        {
            Nome = "Filtro de Óleo",
            Descricao = "Insumo para manutenção preventiva",
            CustoUnitario = 45.90m
        };

        // act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_get_GetById_OK()
    {
        // arrange
        Guid insumoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var admin = context.Usuarios.First();
            var insumo = new InsumoDbModel(
                Guid.NewGuid(), "Pastilha de Freio", "Peça cerâmica", 120.00m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Insumos.Add(insumo);
            await context.SaveChangesAsync();

            insumoId = insumo.Id;
        }

        // act
        var result = await _client.GetAsync($"{ApiKey}/{insumoId}");

        // assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Put_Update_NoContent()
    {
        // arrange
        Guid insumoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var insumo = new InsumoDbModel(
                Guid.NewGuid(), "Óleo 5W30", "Lubrificante", 50.00m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Insumos.Add(insumo);
            await context.SaveChangesAsync();
            insumoId = insumo.Id;
        }

        var updateRequest = new InsumoRequestDTO
        {
            Nome = "Óleo 5W30 Alterado",
            Descricao = "Nova Descrição",
            CustoUnitario = 55.00m
        };

        // act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{insumoId}", updateRequest);

        // assert
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Delete_Delete_NoContent()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = context.Usuarios.First(u => u.Email == "Admin@email.com");
        // arrange
        var insumo = new InsumoDbModel(
            Guid.NewGuid(), "Parafuso", "desc", 10.0m,
            admin.Id, DateTime.UtcNow, null, null, true
        );
        context.Insumos.Add(insumo);
        context.Estoques.Add(new EstoqueDbModel(
            Guid.NewGuid(), null, insumo.Id, 0, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
        ));
        await context.SaveChangesAsync();

        // ACT
        var response = await _client.DeleteAsync($"{ApiKey}/{insumo.Id}");

        // ASSERT
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Insumo_Post_Create_InvalidData_BadRequest()
    {
        // Arrange
        var request = new InsumoRequestDTO
        {
            Nome = "",
            Descricao = "Insumo sem nome",
            CustoUnitario = -1.00m
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Get_GetById_NonExistent_NotFound()
    {
        // Arrange
        var idAleatorio = Guid.NewGuid();

        // Act
        var result = await _client.GetAsync($"{ApiKey}/{idAleatorio}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Delete_WithActiveStock_BadRequest()
    {
        // Arrange
        Guid insumoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First(u => u.Email == "Admin@email.com");

            var insumo = new InsumoDbModel(
                Guid.NewGuid(), "Insumo Bloqueado", "Possui estoque", 10.0m,
                admin.Id, DateTime.UtcNow, null, null, true
            );
            context.Insumos.Add(insumo);

            context.Estoques.Add(new EstoqueDbModel(
                Guid.NewGuid(), null, insumo.Id, 50, 0, new List<EstoqueHistoricoDbmodel>(), null!, null!, true
            ));

            await context.SaveChangesAsync();
            insumoId = insumo.Id;
        }

        // Act
        var result = await _client.DeleteAsync($"{ApiKey}/{insumoId}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Put_Update_NonExistent_NotFound()
    {
        // Arrange
        var idAleatorio = Guid.NewGuid();
        var updateRequest = new InsumoRequestDTO
        {
            Nome = "Insumo Fantasma",
            Descricao = "Desc",
            CustoUnitario = 10
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{idAleatorio}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Get_GetPaginated_PaginaInvalida_BadRequest()
    {
        // Act 
        var result = await _client.GetAsync($"{ApiKey}?page=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Post_Create_ArgumentException_BadRequest()
    {
        // Arrange 
        var request = new InsumoRequestDTO
        {
            Nome = "Teste de insumo",
            Descricao = "Insumo com valor de custo unitario negativo",
            CustoUnitario = -10.0m
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Put_Update_DadosInvalidos_BadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateRequest = new InsumoRequestDTO
        {
            Nome = "",
            Descricao = "Desc",
            CustoUnitario = -50 
        };

        // Act
        var result = await _client.PutAsJsonAsync($"{ApiKey}/{id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Delete_NaoExistente_NotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var result = await _client.DeleteAsync($"{ApiKey}/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Insumo_Get_GetPaginated_FiltroVazio_PartialContent()
    {
        // Act
        var result = await _client.GetAsync($"{ApiKey}?page=1&pageSize=100");

        // Assert
        Assert.Equal(HttpStatusCode.PartialContent, result.StatusCode);
    }
}
