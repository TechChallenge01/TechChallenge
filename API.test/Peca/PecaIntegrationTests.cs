// API.test/Pecas/PecaIntegrationTests.cs
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Application.Pecas.DTOs.Requests;
using Integration.test.Infrastructure;

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

    private static PecaRequestDTO RequestValido(string nome = "Pastilha de Freio Dianteira")
        => new()
        {
            Nome = nome,
            Descricao = "Peça de reposição original",
            PrecoVenda = 150.00m  
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
    public async Task GetPaginated_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoPecaNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Funcionario Teste", "Funcionario");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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
    public async Task Create_DeveRetornarCreated_QuandoAlmoxarifadoCriaPecaValida()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Almoxarife", "Almoxarifado");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarCreated_QuandoAdminCriaPeca()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var response = await _client.PostAsJsonAsync(
            BaseRoute, RequestValido("Filtro de Óleo"));

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
    public async Task Update_DeveRetornarOkOuNotFound_QuandoAdminAtualiza()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Super User", "Administrador");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestValido("Peca Atualizada"));

        response.StatusCode.Should()
            .Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
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
    public async Task Update_DeveRetornarForbidden_QuandoMecanicoTentaAtualizar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Teste", "Mecanico");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarForbidden_QuandoClienteTentaAtualizar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestValido());

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
    public async Task Delete_DeveRetornarForbidden_QuandoAlmoxarifadoTentaExcluir()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Almoxarife", "Almoxarifado");

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
    public async Task Delete_NaoDeveRetornarForbidden_QuandoAdminExclui()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should()
            .Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_QuandoClienteTentaExcluir()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}