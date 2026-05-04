using API.test.Infrastructure;
using Application.Veiculos.DTOs.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Veiculo;

[Collection("IntegrationTests")]
public class VeiculoIntegrationTests
{
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Veiculo";

    public VeiculoIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
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
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoVeiculoNaoExiste()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");

        var response = await client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarForbidden_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Augusto Mecanico", "Mecanico");

        var response = await client.GetAsync(BaseRoute);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarUnauthorized_SemToken()
    {
        var client = _fixture.CriarClienteSemAutenticacao();

        var response = await client.GetAsync(BaseRoute);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_DeveRetornarOk_QuandoDadosValidos_PorFuncionario()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoModeloOuPlacaVazios()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");

        var request = new VeiculoRequestDTO { Modelo = "", Placa = "" };

        var response = await client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_DeveRetornarOkOuNotFound_QuandoExecutadoPorAdmin()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");
        var id = Guid.NewGuid();

        var response = await client.PutAsJsonAsync($"{BaseRoute}/{id}", RequestValido());

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaCliente()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "João", "Cliente");

        var response = await client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarOkOuNotFound_QuandoExecutadoPorFuncionario()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Rafaela Atendente", "Funcionario");

        var response = await client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.NotFound);
    }
}
