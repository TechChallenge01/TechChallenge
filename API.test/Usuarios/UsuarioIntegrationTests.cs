using API.test.Infrastructure;
using Application.Auth.DTOs.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Usuarios;

[Collection("IntegrationTests")]
public class UsuarioIntegrationTests
{
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Usuario";

    public UsuarioIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
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
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin Master", "Administrador");

        var response = await client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Match(s => s == HttpStatusCode.OK || s == HttpStatusCode.Created);
    }

    [Fact]
    public async Task CriarUsuario_DeveRetornarForbidden_ParaFuncionario()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Atendente", "Funcionario");

        var response = await client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CriarUsuario_DeveRetornarForbidden_ParaMecanico()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Mecanico Silva", "Mecanico");

        var response = await client.PostAsJsonAsync(BaseRoute, RequestValido());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    //[Fact]
    //public async Task CriarUsuario_DeveRetornarUnauthorized_SemToken()
    //{
    //    _fixture.RemoverAutenticacao();

    //    var response = await _client.PostAsJsonAsync(BaseRoute, RequestValido());

    //    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    //}

    [Fact]
    public async Task CriarUsuario_DeveRetornarBadRequest_QuandoDadosInvalidos()
    {
        var client = _fixture.CriarClienteAutenticado(Guid.NewGuid(), "Admin", "Administrador");

        var request = new CriarUsuarioRequestDTO { Nome = "Incompleto" };

        var response = await client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
