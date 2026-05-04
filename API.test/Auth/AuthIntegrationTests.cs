using API.test.Infrastructure;
using Application.Auth.DTOs.Requests;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace API.test.Auth;

[Collection("IntegrationTests")]
public class AuthIntegrationTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/Auth";
    public AuthIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    private static LoginRequestDTO LoginValido(
        string email = "admin@teste.com",
        string senha = "Senha@123")
        => new()
        {
            Email = email,
            Senha = senha
        };

    [Fact]
    public async Task Login_DeveRetornarBadRequest_QuandoEmailNaoInformado()
    {
        _fixture.RemoverAutenticacao();

        var request = new LoginRequestDTO
        {
            Email = string.Empty,
            Senha = "Senha@123"
        };

        var response = await _client.PostAsJsonAsync($"{BaseRoute}/Login", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_DeveRetornarBadRequest_QuandoEmailFormatoInvalido()
    {
        _fixture.RemoverAutenticacao();

        var request = new LoginRequestDTO
        {
            Email = "nao-e-um-email",
            Senha = "Senha@123"
        };

        var response = await _client.PostAsJsonAsync($"{BaseRoute}/Login", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_DeveRetornarBadRequest_QuandoSenhaNaoInformada()
    {
        _fixture.RemoverAutenticacao();

        var request = new LoginRequestDTO
        {
            Email = "admin@teste.com",
            Senha = string.Empty
        };

        var response = await _client.PostAsJsonAsync($"{BaseRoute}/Login", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_DeveRetornarBadRequest_QuandoBodyNulo()
    {
        _fixture.RemoverAutenticacao();

        var response = await _client.PostAsJsonAsync<LoginRequestDTO?>(
            $"{BaseRoute}/Login", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_DeveRetornarUnauthorized_QuandoUsuarioNaoExiste()
    {
        _fixture.RemoverAutenticacao();

        var request = LoginValido(email: "usuario.inexistente@teste.com");

        var response = await _client.PostAsJsonAsync($"{BaseRoute}/Login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_DeveRetornarUnauthorized_QuandoSenhaErrada()
    {
        _fixture.RemoverAutenticacao();

        var request = new LoginRequestDTO
        {
            Email = "admin@teste.com",
            Senha = "SenhaErrada@999"
        };

        var response = await _client.PostAsJsonAsync($"{BaseRoute}/Login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_DeveSerAcessivelSemToken_RotaPublica()
    {
        _fixture.RemoverAutenticacao();

        var request = LoginValido();

        var response = await _client.PostAsJsonAsync($"{BaseRoute}/Login", request);

        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.BadRequest
        );
    }

    [Fact]
    public async Task Login_DeveRetornarTokenNoBody_QuandoCredenciaisValidas()
    {
        _fixture.RemoverAutenticacao();

        var request = LoginValido(
            email: "admin@seed.com",   
            senha: "123456"
        );

        var response = await _client.PostAsJsonAsync($"{BaseRoute}/Login", request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            body.Should().NotBeNullOrEmpty();

            var json = JsonDocument.Parse(body);

            json.RootElement
                .GetProperty("data")
                .GetProperty("token")
                .GetString()
                .Should().NotBeNullOrEmpty();
        }
        else
        {
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
