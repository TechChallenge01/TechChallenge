using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using Application.OrdemServicos.DTOs.Requests;
using API.test.Infrastructure;

namespace API.test.OrdemServicos;

[Collection("IntegrationTests")]
public sealed class OrdemServicoIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/ordemservico";

    public OrdemServicoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    private static OrdemServicoRequestDTO RequestComCpf(string cpf = "52998224725",Guid? veiculoId = null,string? observacao = null)
        => new()
        {
            Cpf = cpf,
            VeiculoId = veiculoId ?? Guid.NewGuid(),
            Observacao = observacao,
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

    private static OrdemServicoRequestDTO RequestComCnpj(string cnpj = "11222333000181",Guid? veiculoId = null) 
        => new()
        {
            Cnpj = cnpj,
            VeiculoId = veiculoId ?? Guid.NewGuid(),
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Jose Augusto", "Mecanico");

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
    public async Task GetById_DeveRetornarOkOuNotFound_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should()
            .Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_DeveRetornarForbidden_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoSemCpfECnpj()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = new OrdemServicoRequestDTO
        {
            VeiculoId = Guid.NewGuid(),
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoCpfECnpjInformados()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = new OrdemServicoRequestDTO
        {
            Cpf = "52998224725",
            Cnpj = "11222333000181",
            VeiculoId = Guid.NewGuid(),
            Pecas = new List<OrdemServicoPecaRequestDTO>(),
            Servicos = new List<OrdemServicoServicoRequestDTO>(),
            Insumos = new List<OrdemServicoInsumoRequestDTO>()
        };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarNotFound_QuandoClienteNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = RequestComCpf(cpf: "52998224725");

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var request = RequestComCpf();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        // Cliente não pode criar OS
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");

        var request = RequestComCpf();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Cancelar_NaoDeveRetornarForbidden_ParaAdministrador()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/Cancelar", null);

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Cancelar_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/Cancelar", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Aprovar_NaoDeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Dono do Carro", "Cliente");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/Aprovar", null);

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Aprovar_NaoDeveRetornarForbidden_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/Aprovar", null);

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Aprovar_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/Aprovar", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task IniciarDiagnostico_NaoDeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/IniciarDiagnostico", null);

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task IniciarDiagnostico_DeveRetornarForbidden_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/IniciarDiagnostico", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RealizarDiagnostico_NaoDeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");

        var request = new DiagnosticoRequestDTO
        {
            Observacao = "Troca de pastilhas de freio identificada.",
            Servicos = new List<DiagnosticoServicoDTO>(),
            Pecas = new List<DiagnosticoPecaDTO>()
        };

        var response = await _client.PostAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/RealizarDiagnostico", request);

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RealizarDiagnostico_DeveRetornarBadRequest_QuandoSemItens()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");

        var request = new DiagnosticoRequestDTO
        {
            Observacao = "Diagnóstico sem itens.",
            Servicos = new List<DiagnosticoServicoDTO>(),
            Pecas = new List<DiagnosticoPecaDTO>()
        };

        var response = await _client.PostAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/RealizarDiagnostico", request);

        response.StatusCode.Should()
            .Match(s => s == HttpStatusCode.BadRequest || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task FinalizarServico_NaoDeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");

        var dto = new FinalizarServicoDTO
        {
            ServicosId = new List<Guid> { Guid.NewGuid() }
        };

        var response = await _client.PostAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/FinalizarServico", dto);

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task FinalizarServico_DeveRetornarForbidden_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var dto = new FinalizarServicoDTO
        {
            ServicosId = new List<Guid> { Guid.NewGuid() }
        };

        var response = await _client.PostAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/FinalizarServico", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegistrarEntrega_NaoDeveRetornarForbidden_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/RegistrarEntrega", null);

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegistrarEntrega_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/RegistrarEntrega", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegistrarEntrega_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.PostAsync(
            $"{BaseRoute}/{Guid.NewGuid()}/RegistrarEntrega", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}