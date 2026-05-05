using Application.Estoques.DTOs.Requests;
using Domain.Enums;
using Infra.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace API.test.Estoque;

public class EstoqueTest : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    const string ApiKey = "api/Estoque";

    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    private readonly IntegrationTestFixture _fixture;
    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    public EstoqueTest(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _factory = fixture.App;
    }

    [Fact]
    public async Task Estoque_Post_Entrada_Insumo_Created()
    {
        // Arrange
        Guid insumoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var insumo = new Insumo("Óleo", "Sintético 5W30", 60m, admin.Id, DateTime.UtcNow);
            context.Insumos.Add(insumo);

            var estoque = new Domain.Aggregates.EstoqueAggregates.Estoque(insumo.Id, null, 10, admin.Id, DateTime.UtcNow);
            context.Estoques.Add(estoque);
            await context.SaveChangesAsync();
            insumoId = insumo.Id;
        }

        var request = new EstoqueRequestDTO
        {
            InsumoId = insumoId,
            TipoMovimentacao = "Entrada",
            Quantidade = 5
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Post_Saida_Insumo_Created()
    {
        // Arrange
        Guid insumoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var insumo = new Insumo("Fluido Freio", "DOT4", 40m, admin.Id, DateTime.UtcNow);
            context.Insumos.Add(insumo);

            var estoque = new Domain.Aggregates.EstoqueAggregates.Estoque(insumo.Id, null, 50, admin.Id, DateTime.UtcNow);
            context.Estoques.Add(estoque);
            await context.SaveChangesAsync();
            insumoId = insumo.Id;
        }

        var request = new EstoqueRequestDTO
        {
            InsumoId = insumoId,
            TipoMovimentacao = "Saida",
            Quantidade = 10
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Post_Entrada_Peca_Created()
    {
        // Arrange
        Guid pecaId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var peca = new Domain.Entities.Peca("Pastilha Freio", "Cerâmica", "Brembo", 250m, admin.Id, DateTime.UtcNow);
            context.Pecas.Add(peca);

            var estoque = new Domain.Aggregates.EstoqueAggregates.Estoque(null, peca.Id, 0, admin.Id, DateTime.UtcNow);
            context.Estoques.Add(estoque);
            await context.SaveChangesAsync();
            pecaId = peca.Id;
        }

        var request = new EstoqueRequestDTO
        {
            PecaId = pecaId,
            TipoMovimentacao = "Entrada",
            Quantidade = 12
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Post_Saida_Peca_Created()
    {
        // Arrange
        Guid pecaId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var peca = new Domain.Entities.Peca("Filtro Ar", "Esportivo", "K&N", 180m, admin.Id, DateTime.UtcNow);
            context.Pecas.Add(peca);

            var estoque = new Domain.Aggregates.EstoqueAggregates.Estoque(null, peca.Id, 100, admin.Id, DateTime.UtcNow);
            context.Estoques.Add(estoque);
            await context.SaveChangesAsync();
            pecaId = peca.Id;
        }

        var request = new EstoqueRequestDTO
        {
            PecaId = pecaId,
            TipoMovimentacao = "Saida",
            Quantidade = 5
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Post_Movimentar_AmbosIdsPreenchidos_BadRequest()
    {
        // Arrange
        var request = new EstoqueRequestDTO
        {
            InsumoId = Guid.NewGuid(),
            PecaId = Guid.NewGuid(),
            TipoMovimentacao = "Entrada",
            Quantidade = 10
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Post_Movimentar_NenhumIdPreenchido_BadRequest()
    {
        // Arrange
        var request = new EstoqueRequestDTO
        {
            TipoMovimentacao = "Entrada",
            Quantidade = 10
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Post_Movimentar_TipoInvalido_BadRequest()
    {
        // Arrange
        var request = new EstoqueRequestDTO
        {
            InsumoId = Guid.NewGuid(),
            TipoMovimentacao = "Inexistente",
            Quantidade = 10
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Post_Movimentar_EstoqueNaoEncontrado_NotFound()
    {
        // Arrange
        Guid insumoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();
            var insumo = new Insumo("Teste", "Desc", 10m, admin.Id, DateTime.UtcNow);
            context.Insumos.Add(insumo);
            await context.SaveChangesAsync();
            insumoId = insumo.Id;
        }

        var request = new EstoqueRequestDTO
        {
            InsumoId = insumoId,
            TipoMovimentacao = "Entrada",
            Quantidade = 1
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Post_Saida_Insumo_SaldoInsuficiente_BadRequest()
    {
        // Arrange
        Guid insumoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var admin = context.Usuarios.First();

            var insumo = new Insumo("Óleo", "Desc", 50m, admin.Id, DateTime.UtcNow);
            context.Insumos.Add(insumo);

            var estoque = new Domain.Aggregates.EstoqueAggregates.Estoque(insumo.Id, null, 5, admin.Id, DateTime.UtcNow);
            context.Estoques.Add(estoque);
            await context.SaveChangesAsync();
            insumoId = insumo.Id;
        }

        var request = new EstoqueRequestDTO
        {
            InsumoId = insumoId,
            TipoMovimentacao = "Saida",
            Quantidade = 10 
        };

        // Act
        var result = await _client.PostAsJsonAsync(ApiKey, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Get_GetPaginated_PaginaInvalida_BadRequest()
    {
        // Act
        var result = await _client.GetAsync($"{ApiKey}?page=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Estoque_Get_GetById_NonExistent_NotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var result = await _client.GetAsync($"{ApiKey}/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}
