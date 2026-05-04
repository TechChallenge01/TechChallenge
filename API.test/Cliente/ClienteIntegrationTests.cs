using API.test.Infrastructure;
using Application.Clientes.DTOs.Requests;
using Application.Clientes.DTOs.Shared;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Cliente;

[Collection("IntegrationTests")]
public class ClienteIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Cliente";
    public ClienteIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    private static ClienteRequestDTO RequestComCpfValido(
        string nome = "João da Silva",
        string cpf = "52998224725")
        => new()
        {
            Nome = nome,
            Cpf = cpf,
            Email = $"{nome.Replace(" ", "").ToLower()}@teste.com",
            Telefone = new TelefoneDTO
            {
                DDD = "11",
                DDI = "55",
                Numero = "987654321",
            },
            Enderecos = new EnderecoDTO
            {
                Logradouro = "Rua Teste",
                Numero = "123",
                Complemento = "Apto 1",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Uf = "SP",
                Cep = "01310100"
            }
        };

    private static ClienteRequestDTO RequestComCnpjValido(
        string nome = "Empresa Teste LTDA",
        string cnpj = "11222333000181")
        => new()
        {
            Nome = nome,
            Cnpj = cnpj,
            Email = "empresa@teste.com",
            Telefone = new TelefoneDTO
            {
                DDD = "11",
                DDI = "55",
                Numero = "33334444"
            }
        };

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarOk_ParaAdministrador()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Administrador", "Administrador");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPaginated_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoClienteNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoSemCpfECnpj()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = new ClienteRequestDTO
        {
            Nome = "Sem Documento",
            Email = "semdoc@teste.com",
            Telefone = new TelefoneDTO
            {
                DDD = "11",
                DDI = "55",
                Numero = "987654321"
            }
        };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoCpfECnpjInformados()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = new ClienteRequestDTO
        {
            Nome = "Ambos Docs",
            Cpf = "52998224725",
            Cnpj = "11222333000181",
            Email = "ambos@teste.com",
            Telefone = new TelefoneDTO
            {
                DDD = "11",
                DDI = "55",
                Numero = "987654321"
            }
        };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarBadRequest_QuandoNomeVazio()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = RequestComCpfValido(nome: string.Empty);

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornarCreated_QuandoClienteComCpfValido()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var request = RequestComCpfValido();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarCreated_QuandoClienteComCnpjValido()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Administrador", "Administrador");

        var request = RequestComCnpjValido();

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestComCpfValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestComCpfValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestComCpfValido());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_DeveRetornarNotFound_QuandoClienteNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestComCpfValido(nome: "Nome Atualizado"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestComCpfValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.PutAsJsonAsync(
            $"{BaseRoute}/{Guid.NewGuid()}",
            RequestComCpfValido());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoClienteNaoExiste()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico", "Mecanico");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarForbidden_ParaCliente()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Carlos", "Cliente");

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
