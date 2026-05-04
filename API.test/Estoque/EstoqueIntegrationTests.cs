using API.test.Infrastructure;
using Application.Estoques.DTOs.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Estoque;

// Removido IClassFixture — quando usa [Collection], o fixture vem da collection
[Collection("IntegrationTests")]
public class EstoqueIntegrationTests
{
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Estoque";

    public EstoqueIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
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
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

        var response = await client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoEstoqueNaoExiste()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");

        var response = await client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarUnauthorized_SemToken()
    {
        var client = _fixture.CriarClienteSemAutenticacao();

        var response = await client.GetAsync(BaseRoute);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Movimentar_DeveRetornarBadRequest_QuandoQuantidadeForZeroOuNegativa()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = new EstoqueRequestDTO
        {
            PecaId = Guid.NewGuid(),
            TipoMovimentacao = "Entrada",
            Quantidade = 0
        };

        var response = await client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Movimentar_DeveRetornarForbidden_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var request = RequestMovimentacaoValida();

        var response = await client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Movimentar_DeveRetornarUnauthorized_SemToken()
    {
        var client = _fixture.CriarClienteSemAutenticacao();

        var request = RequestMovimentacaoValida();

        var response = await client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

}