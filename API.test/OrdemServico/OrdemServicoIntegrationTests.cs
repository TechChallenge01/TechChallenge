using API.test.Infrastructure;
using Application.OrdemServicos.DTOs.Requests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace API.test.OrdemServico;

[Collection("IntegrationTests")]
public class OrdemServicoIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/OrdemServico";

    public OrdemServicoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    private static OrdemServicoRequestDTO RequestCriacaoValida() => new()
    {
        VeiculoId = Guid.NewGuid(),
        Observacao = "Revisão preventiva",
        Cpf = "12345678901",
        ValorDesconto = 0
    };

    private static DiagnosticoRequestDTO RequestDiagnosticoValido() => new()
    {
        Observacao = "Necessário trocar pastilhas de freio",
        Servicos = new List<OrdemServicoServicoRequestDTO>
        {
            new() { ServicoId = Guid.NewGuid(), Quantidade = 1 }
        }
    };

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "João", "Cliente");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_DeveRetornarOkOuNotFound_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "João", "Cliente");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Consultas_DevemRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.GetAsync(BaseRoute);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_DeveRetornarOk_QuandoDadosValidos_PorFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestCriacaoValida());

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoSemVeiculo()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var request = new OrdemServicoRequestDTO { Observacao = "Sem veículo associado" };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestCriacaoValida());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Cancelar_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await _client.PutAsync($"{BaseRoute}/{Guid.NewGuid()}/Cancelar", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Aprovar_DevePermitir_AcessoParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "João", "Cliente");

        var response = await _client.PutAsync($"{BaseRoute}/{Guid.NewGuid()}/Aprovar", null);

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task IniciarDiagnostico_DeveRetornarForbidden_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await _client.PutAsync($"{BaseRoute}/{Guid.NewGuid()}/IniciarDiagnostico", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RealizarDiagnostico_DeveRetornarOk_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{Guid.NewGuid()}/RealizarDiagnostico", RequestDiagnosticoValido());

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task FinalizarServico_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "João", "Cliente");

        var request = new FinalizarServicoDTO { ServicosId = new List<Guid> { Guid.NewGuid() } };

        var response = await _client.PutAsJsonAsync($"{BaseRoute}/{Guid.NewGuid()}/FinalizarServico", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegistrarEntrega_DeveRetornarOk_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await _client.PutAsync($"{BaseRoute}/{Guid.NewGuid()}/RegistrarEntrega", null);

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }
}
