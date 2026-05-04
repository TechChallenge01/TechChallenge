using API.test.Infrastructure;
using Application.Veiculos.DTOs.Requests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace API.test.Veiculo;

[Collection("IntegrationTests")]
public class VeiculoIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Veiculo";

    public VeiculoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    private static VeiculoRequestDTO RequestValido(Guid? clienteId = null) => new()
    {
        MarcaVeiculo = "Toyota",
        Modelo = "Corolla",
        Ano = 2024,
        Placa = "ABC1D23",
        Cor = "Prata",
        ClienteId = clienteId ?? Guid.NewGuid()
    };

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoVeiculoNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await _client.GetAsync(BaseRoute);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoModeloOuPlacaVazios()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var request = new VeiculoRequestDTO { Modelo = "", Placa = "" };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_DeveRetornarOkOuNotFound_QuandoExecutadoPorAdmin()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");
        var id = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{id}", RequestValido());

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "João", "Cliente");

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
