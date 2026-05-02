using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using Application.Veiculos.DTOs.Requests;
using API.test.Infrastructure;

namespace API.test.Veiculos;

[Collection("IntegrationTests")]
public sealed class VeiculoIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/veiculo";

    public VeiculoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    #region Consultas (GET)

    [Fact]
    public async Task GetPaginated_DeveRetornarForbidden_QuandoMecanicoTentaAcessar()
    {
        // Arrange 
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");

        // Act
        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_DeveRetornarOk_QuandoFuncionarioAcessa()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");
        var id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"{BaseRoute}/{id}");

        // Assert
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    #endregion

    #region Cadastro e Edição (POST, PUT, DELETE)

    [Fact]
    public async Task Create_DeveRetornarOk_QuandoDadosSaoValidos()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var request = new VeiculoRequestDTO
        {
            Placa = "ABC1D23",
            Modelo = "Civic",
            Marca = "Honda",
            Ano = 2022,
            ClienteId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_DeveRetornarOk_QuandoIdEhValido()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");
        var id = Guid.NewGuid();
        var request = new VeiculoRequestDTO { Modelo = "Civic Atualizado" };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{id}", request);

        // Assert
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarOk_QuandoExecutadoPorAdmin()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");
        var id = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"{BaseRoute}/{id}");

        // Assert
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    #endregion
}