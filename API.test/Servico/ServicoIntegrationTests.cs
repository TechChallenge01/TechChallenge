// API.test/Servicos/ServicoIntegrationTests.cs
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Application.Servicos.DTOs.Requests;
using Integration.test.Infrastructure;

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

    private static ServicoRequestDTO RequestValido(string nome = "Alinhamento e Balanceamento")
        => new()
        {
            Nome = nome,
            Descricao = "Serviço completo de geometria veicular",
            PrecoVenda = 180.00m
        };

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

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
    public async Task GetById_DeveRetornarNotFound_QuandoServicoNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_DeveRetornarOk_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

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
    public async Task Create_DeveRetornarCreated_QuandoFuncionarioCriaServicoValido()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarCreated_QuandoAdminCriaServico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido("Troca de Óleo"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_QuandoMecanicoTentaCriar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

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
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestValido("Revisão de Freios Alterada"));

        response.StatusCode.Should()
            .Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DeveRetornarForbidden_QuandoMecanicoTentaAtualizar()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

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
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

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