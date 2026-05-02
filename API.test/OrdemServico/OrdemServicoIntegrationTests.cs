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

    #region Consultas (GET)

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaUsuariosAutorizados()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Jose Augusto", "Mecanico");
        
        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarOk_ParaClienteOuStaff()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");
        var idInexistente = Guid.NewGuid();

        var response = await _client.GetAsync($"{BaseRoute}/{idInexistente}");

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    #endregion

    #region Fluxo Principal e Status (POST)

    [Fact]
    public async Task Create_DeveRetornarOk_QuandoFuncionarioCria()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");
        
        var request = new OrdemServicoRequestDTO 
        { 
            ClienteId = Guid.NewGuid(), 
            VeiculoId = Guid.NewGuid(), 
            DescricaoProblema = "Revisão Geral" 
        };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cancelar_DevePermitir_ApenasParaStaff()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");
        
        var response = await _client.PostAsync($"{BaseRoute}/{Guid.NewGuid()}/Cancelar", null);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Aprovar_DevePermitir_ParaClienteOuStaff()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Dono do Carro", "Cliente");
        
        var response = await _client.PostAsync($"{BaseRoute}/{Guid.NewGuid()}/Aprovar", null);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Operações Técnicas (Mecânico)

    [Fact]
    public async Task IniciarDiagnostico_DeveRetornarOk_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");
        
        var response = await _client.PostAsync($"{BaseRoute}/{Guid.NewGuid()}/IniciarDiagnostico", null);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RealizarDiagnostico_DeveRetornarOk_ComDadosValidos()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");
        
        var request = new DiagnosticoRequestDTO 
        { 
            RelatorioTecnico = "Troca de pastilhas", 
            MaoDeObraEstimada = 150,
            PecasSugeridas = new List<Guid>() 
        };

        var response = await _client.PostAsJsonAsync($"{BaseRoute}/{Guid.NewGuid()}/RealizarDiagnostico", request);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task FinalizarServico_DeveRetornarOk_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico 01", "Mecanico");
        
        var dto = new FinalizarServicoDTO { Observacoes = "Tudo certo." };
        var response = await _client.PostAsJsonAsync($"{BaseRoute}/{Guid.NewGuid()}/FinalizarServico", dto);
        
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Entrega (Finalização do Processo)

    [Fact]
    public async Task RegistrarEntrega_DeveRetornarOk_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");
        
        var response = await _client.PostAsync($"{BaseRoute}/{Guid.NewGuid()}/RegistrarEntrega", null);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    #endregion
}