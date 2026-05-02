// API.test/Insumos/InsumoIntegrationTests.cs
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

    private static InsumoRequestDTO RequestValido(string nome = "Óleo de Motor 5W30")
        => new()
        {
            Nome = nome,
            Descricao = "Insumo para troca de óleo",
            UnidadeMedida = "Litro",
            PrecoUnitario = 45.90m
        };

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Teste", "Mecanico");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaAlmoxarifado()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Almoxarife", "Almoxarifado");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoInsumoNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Almoxarife", "Almoxarifado");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_NaoDeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Teste", "Mecanico");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should()
            .Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_DeveRetornarCreated_QuandoAlmoxarifadoCriaInsumoValido()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Almoxarife", "Almoxarifado");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarCreated_QuandoAdminCriaInsumo()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin Master", "Administrador");

        var response = await _client.PostAsJsonAsync(
            BaseRoute, RequestValido("Fluido de Freio DOT4"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_QuandoMecanicoTentaCriar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Teste", "Mecanico");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_QuandoFuncionarioTentaCriar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_QuandoClienteTentaCriar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarForbidden_QuandoMecanicoTentaEditar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Teste", "Mecanico");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestValido("Tentativa de Update"));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarForbidden_QuandoFuncionarioTentaEditar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_NaoDeveRetornarForbidden_QuandoAlmoxarifadoAtualiza()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Almoxarife", "Almoxarifado");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestValido());

        response.StatusCode.Should()
            .Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_NaoDeveRetornarForbidden_QuandoAdminAtualiza()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin Master", "Administrador");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestValido("Insumo Atualizado"));

        response.StatusCode.Should()
            .Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DeveRetornarForbidden_QuandoClienteTentaEditar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_NaoDeveRetornarForbidden_QuandoAdministradorDeleta()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin Master", "Administrador");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");
        response.StatusCode.Should()
            .Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_QuandoAlmoxarifadoTentaExcluir()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Almoxarife", "Almoxarifado");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_QuandoMecanicoTentaExcluir()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Teste", "Mecanico");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_QuandoFuncionarioTentaExcluir()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_QuandoClienteTentaExcluir()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}