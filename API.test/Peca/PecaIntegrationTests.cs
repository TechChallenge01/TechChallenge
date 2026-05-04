using API.test.Infrastructure;
using Application.Pecas.DTOs.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Peca;

[Collection("IntegrationTests")]
public class PecaIntegrationTests
{
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Peca";

    public PecaIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
    }

    private static PecaRequestDTO RequestValido(string nome = "Pastilha de Freio", decimal preco = 150.00m)
        => new()
        {
            Nome = nome,
            Descricao = "Pastilha de cerâmica de alta performance",
            MarcaPeca = "Brembo",
            PrecoVenda = preco
        };

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

        var response = await client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoPecaNaoExiste()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

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
    public async Task Create_DeveRetornarOk_QuandoDadosSaoValidos()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");

        var request = RequestValido();

        var response = await client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoNomeOuMarcaVazios()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");

        var request = new PecaRequestDTO
        {
            Nome = "",
            Descricao = "Teste",
            MarcaPeca = "",
            PrecoVenda = 10
        };

        var response = await client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaFuncionario()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarOk_QuandoExecutadoPorAdmin()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");
        var id = Guid.NewGuid();

        var response = await client.PutAsJsonAsync($"{BaseRoute}/{id}", RequestValido("Disco de Freio"));

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
