using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using Application.Estoques.DTOs.Requests;
using Application.Estoques.DTOs.Responses;
using API.test.Infrastructure;

namespace API.test.Estoques;

[Collection("IntegrationTests")]
public sealed class EstoqueIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/estoque";

    public EstoqueIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;

        _fixture.AutenticarClient(Guid.NewGuid(), "User Teste", "Administrador");
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_QuandoUsuarioAutenticado()
    {
        // Act
        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Movimentar_DeveRetornarOk_QuandoDadosValidos()
    {
        // Arrange
        var request = new EstoqueRequestDTO
        {
            PecaId = Guid.NewGuid(), 
            Quantidade = 10,
            TipoMovimentacao = 1, 
            Observacao = "Entrada de teste via integração"
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_ComIdInexistente_DeveRetornarNotFound()
    {
        // Act
        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}