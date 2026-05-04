using API.test.Infrastructure;
using Application.Estoques.DTOs.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Estoque;

[Collection("IntegrationTests")]
public class EstoqueIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Estoque";

    public EstoqueIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    private static EstoqueRequestDTO RequestMovimentacaoValida(Guid? pecaId = null, Guid? insumoId = null)
        => new()
        {
            PecaId = pecaId ?? Guid.NewGuid(),
            InsumoId = insumoId,
            TipoMovimentacao = "Entrada",
            Quantidade = 10
        };

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaQualquerMembroDaEquipe()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoEstoqueNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.GetAsync($"{BaseRoute}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Movimentar_DeveRetornarOk_QuandoDadosSaoValidos()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");

        var request = RequestMovimentacaoValida();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Movimentar_DeveRetornarBadRequest_QuandoQuantidadeForZeroOuNegativa()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = new EstoqueRequestDTO
        {
            PecaId = Guid.NewGuid(),
            TipoMovimentacao = "Entrada",
            Quantidade = 0 
        };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Movimentar_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var request = RequestMovimentacaoValida();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Movimentar_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var request = RequestMovimentacaoValida();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
