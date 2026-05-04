using API.test.Infrastructure;
using Application.Auth.DTOs.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Usuario;

[Collection("IntegrationTests")]
public class UsuarioIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Usuario";

    public UsuarioIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    private static CriarUsuarioRequestDTO RequestValido() => new()
    {
        Nome = "Novo Usuario Teste",
        Email = "novo.usuario@oficina.com",
        Senha = "Senha@Forte123",
        Perfil = Domain.Enums.EPerfilUsuario.Mecanico
    };

    [Fact]
    public async Task CriarUsuario_DeveRetornarOk_QuandoExecutadoPorAdministrador()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin Master", "Administrador");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.Created);
    }

    [Fact]
    public async Task CriarUsuario_DeveRetornarForbidden_ParaFuncionario()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CriarUsuario_DeveRetornarForbidden_ParaMecanico()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CriarUsuario_DeveRetornarUnauthorized_SemToken()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriarUsuario_DeveRetornarBadRequest_QuandoDadosInvalidos()
    {
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin", "Administrador");

        var request = new CriarUsuarioRequestDTO { Nome = "Incompleto" };

        var response = await _client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
