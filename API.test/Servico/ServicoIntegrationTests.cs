using API.test.Infrastructure;
using Application.Servicos.DTOs.Requests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace API.test.Servico;

[Collection("IntegrationTests")]
public class ServicoIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Servico";

    public ServicoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
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
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoServicoNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.GetAsync(BaseRoute);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_DeveRetornarOk_QuandoDadosValidos_PorFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var request = RequestValido();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoNomeVazio()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var request = new ServicoRequestDTO
        {
            Nome = string.Empty,
            Descricao = "Teste",
            PrecoVenda = 100
        };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarOkOuNotFound_QuandoExecutadoPorAdmin()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");
        var id = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{id}", RequestValido("Troca de Óleo"));

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{Guid.NewGuid()}", RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarOkOuNotFound_QuandoExecutadoPorFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }
}
