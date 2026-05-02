using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using Application.Insumos.DTOs.Requests;
using API.test.Infrastructure;

namespace API.test.Insumos;

[Collection("IntegrationTests")]
public sealed class InsumoIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/insumo";

    public InsumoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    #region Consultas (GET)

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaMecanicoOuAlmoxarifado()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Teste", "Mecanico");

        // Act
        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Escrita e Manutenção (POST, PUT, DELETE)

    [Fact]
    public async Task Create_DeveRetornarOk_QuandoAlmoxarifadoCriaInsumoValido()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Almoxarife", "Almoxarifado");

        var request = new InsumoRequestDTO
        {
            Nome = "Óleo de Motor 5W30",
            Descricao = "Insumo para troca de óleo",
            UnidadeMedida = "Litro",
            PrecoUnitario = 45.90m
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_DeveRetornarForbidden_QuandoMecanicoTentaEditar()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");
        var insumoId = Guid.NewGuid();
        var request = new InsumoRequestDTO { Nome = "Tentativa de Update" };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{insumoId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarOk_QuandoAdministradorDeleta()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin Master", "Administrador");
        var insumoId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"{BaseRoute}/{insumoId}");

        // Assert
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    #endregion
}