using API.test.Infrastructure;
using Application.Insumos.DTOs.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Insumo;

[Collection("IntegrationTests")]
public class InsumoIntegrationTests
{
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Insumo";

    public InsumoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
    }

    private static InsumoRequestDTO RequestValido(string nome = "Óleo 5W30", decimal custo = 45.50m)
        => new()
        {
            Nome = nome,
            Descricao = "Insumo para manutenção de motor",
            CustoUnitario = custo
        };

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Mecanico Teste", "Mecanico");
        var response = await client.GetAsync($"{BaseRoute}?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoInsumoNaoExiste()
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
    public async Task Create_DeveRetornarOk_QuandoDadosSaoValidos()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");
        var response = await client.PostAsJsonAsync(BaseRoute, RequestValido());
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoNomeVazio()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");
        var request = new InsumoRequestDTO { Nome = "", CustoUnitario = 10 };
        var response = await client.PostAsJsonAsync(BaseRoute, request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaFuncionarioOuMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Atendente", "Funcionario");
        var response = await client.PostAsJsonAsync(BaseRoute, RequestValido());
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarOkOuNotFound_QuandoExecutadoPorAlmoxarifado()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");
        var response = await client.PutAsJsonAsync($"{BaseRoute}/{Guid.NewGuid()}", RequestValido("Nome Atualizado"));
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Mecanico", "Mecanico");
        var response = await client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarOkOuNotFound_QuandoExecutadoPorAdmin()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");
        var response = await client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");
        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }
}