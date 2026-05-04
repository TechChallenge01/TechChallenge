using API.test.Infrastructure;
using Application.Servicos.DTOs.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Servico;

[Collection("IntegrationTests")]
public class ServicoIntegrationTests
{
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Servico";

    public ServicoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
    }

    private static ServicoRequestDTO RequestValido(string nome = "Alinhamento e Balanceamento", decimal preco = 200.00m)
        => new()
        {
            Nome = nome,
            Descricao = "Ajuste da suspensão e balanceamento das rodas",
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
    public async Task GetById_DeveRetornarNotFound_QuandoServicoNaoExiste()
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
    public async Task Create_DeveRetornarOk_QuandoDadosValidos_PorFuncionario()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var request = RequestValido();

        var response = await client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoNomeVazio()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");

        var request = new ServicoRequestDTO
        {
            Nome = string.Empty,
            Descricao = "Teste",
            PrecoVenda = 100
        };

        var response = await client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarOkOuNotFound_QuandoExecutadoPorAdmin()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");
        var id = Guid.NewGuid();

        var response = await client.PutAsJsonAsync($"{BaseRoute}/{id}", RequestValido("Troca de Óleo"));

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DeveRetornarForbidden_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await client.PutAsJsonAsync($"{BaseRoute}/{Guid.NewGuid()}", RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarOkOuNotFound_QuandoExecutadoPorFuncionario()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }
}
