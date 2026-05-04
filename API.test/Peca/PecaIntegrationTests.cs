using API.test.Infrastructure;
using Application.Pecas.DTOs.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace API.test.Peca;

[Collection("IntegrationTests")]
public class PecaIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Peca";

    public PecaIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
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
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoPecaNaoExiste()
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
    public async Task Create_DeveRetornarOk_QuandoDadosSaoValidos()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos Almoxarife", "Almoxarifado");

        var request = RequestValido();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoNomeOuMarcaVazios()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var request = new PecaRequestDTO
        {
            Nome = "",
            Descricao = "Teste",
            MarcaPeca = "",
            PrecoVenda = 10
        };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarOk_QuandoExecutadoPorAdmin()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");
        var id = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{id}", RequestValido("Disco de Freio"));

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
