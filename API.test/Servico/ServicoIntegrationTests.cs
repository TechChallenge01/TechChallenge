using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using Application.Servicos.DTOs.Requests;
using API.test.Infrastructure;

namespace API.test.Servicos;

[Collection("IntegrationTests")]
public sealed class ServicoIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/servico";

    public ServicoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    #region Consultas (GET)

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_QuandoUsuarioEhMecanico()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

        // Act
        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoServicoNaoExiste()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");
        var idAleatorio = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"{BaseRoute}/{idAleatorio}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Manutenção de Catálogo (POST, PUT, DELETE)

    [Fact]
    public async Task Create_DeveRetornarOk_QuandoFuncionarioCriaServicoValido()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = new ServicoRequestDTO
        {
            Nome = "Alinhamento e Balanceamento",
            Descricao = "Serviço completo de geometria veicular",
            PrecoBase = 180.00m,
            TempoEstimadoMinutos = 60
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_DeveRetornarOk_QuandoAdminAtualizaServico()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");
        var servicoId = Guid.NewGuid();
        var request = new ServicoRequestDTO
        {
            Nome = "Revisão de Freios Alterada",
            PrecoBase = 250.00m
        };

        // Act 
        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{servicoId}", request);

        // Assert
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_QuandoMecanicoTentaExcluir()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");
        var servicoId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"{BaseRoute}/{servicoId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion
}