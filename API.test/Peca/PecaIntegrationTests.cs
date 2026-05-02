using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using Application.Pecas.DTOs.Requests;
using API.test.Infrastructure;

namespace API.test.Pecas;

[Collection("IntegrationTests")]
public sealed class PecaIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/peca";

    public PecaIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    #region Consultas (GET)

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaQualquerMembroDaEquipe()
    {
        // Arrange 
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Teste", "Mecanico");

        // Act
        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoPecaNaoExiste()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Funcionario Teste", "Funcionario");
        var idInexistente = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"{BaseRoute}/{idInexistente}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Escrita e Manutenção (POST, PUT, DELETE)

    [Fact]
    public async Task Create_DeveRetornarOk_QuandoAlmoxarifadoCriaPeca()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Almoxarife", "Almoxarifado");

        var request = new PecaRequestDTO
        {
            Nome = "Pastilha de Freio Dianteira",
            CodigoFabricante = "PF-12345",
            PrecoCusto = 80.00m,
            PrecoVenda = 150.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_DeveRetornarOk_QuandoUsarRoleAdmin()
    {
        // Arrange 
        _fixture.AutenticarClient(Guid.NewGuid(), "Super User", "Administrador");

        var pecaId = Guid.NewGuid();
        var request = new PecaRequestDTO { Nome = "Peca Atualizada" };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{pecaId}", request);

        // Assert
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_QuandoMecanicoTentaExcluir()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");
        var pecaId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"{BaseRoute}/{pecaId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion
}