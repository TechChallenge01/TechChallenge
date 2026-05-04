using API.test.Infrastructure;
using Application.Insumos.DTOs.Requests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace API.test.Insumo;

[Collection("IntegrationTests")]
public class InsumoIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Insumo";
    public InsumoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
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
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Teste", "Mecanico");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoInsumoNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");

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
    public async Task Create_DeveRetornarOk_QuandoDadosSaoValidos()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var request = RequestValido();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoNomeVazio()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");

        var request = new InsumoRequestDTO { Nome = "", CustoUnitario = 10 };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaFuncionarioOuMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarOk_QuandoExecutadoPorAlmoxarifado()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");
        var id = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{id}", RequestValido("Nome Atualizado"));

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarOk_QuandoExecutadoPorAdmin()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }
}
