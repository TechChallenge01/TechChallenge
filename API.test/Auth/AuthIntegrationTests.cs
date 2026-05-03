using System.Net;
using System.Net.Http.Json;
using Application.Auth.DTOs.Requests;
using FluentAssertions;
using Integration.test.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Infra.Context;
using Domain.Entities;
using Xunit;
using Domain.Enums;

namespace Integration.test.Auth;

[Collection("IntegrationTests")]
public sealed class AuthIntegrationTests
{
    private readonly HttpClient _client;
    private readonly IntegrationTestBase _fixture;
    private const string BaseRoute = "/api/auth";

    public AuthIntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    public async Task Post_LoginComCredenciaisValidas_RetornaOk()
    {
        // Arrange - Criar um usuário diretamente no banco para poder logar
        var email = "login.sucesso@teste.com";
        var senha = "SenhaSegura123!";
        await CriarUsuarioNoBancoAsync(email, senha);

        var request = new LoginRequestDTO { Email = email, Senha = senha };

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseRoute}/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_LoginComSenhaIncorreta_RetornaUnauthorized()
    {
        // Arrange
        await CriarUsuarioNoBancoAsync("senha.errada@teste.com", "SenhaCorreta");
        var request = new LoginRequestDTO { Email = "senha.errada@teste.com", Senha = "SenhaErrada" };

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseRoute}/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CriarUsuarioComAdminLogado_RetornaCreated()
    {
        // Arrange
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin Global", "Admin");

        var request = new CriarUsuarioRequestDTO
        {
            Nome = "Novo Tecnico",
            Email = "tecnico@oficina.com",
            Senha = "Senha@123",
            Perfil = EPerfilUsuario.Funcionario 
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseRoute}/usuarios", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_CriarUsuarioSemToken_RetornaUnauthorized()
    {
        // Arrange 
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new CriarUsuarioRequestDTO { /* dados */ };

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseRoute}/usuarios", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CriarUsuarioComPerfilMecanico_RetornaForbidden()
    {
        // Arrange 
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Funcionario");

        var request = new CriarUsuarioRequestDTO { /* dados */ };

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseRoute}/usuarios", request);

        // Assert 
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task CriarUsuarioNoBancoAsync(string email, string senha)
    {
        using var scope = _fixture.GetScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (context.Usuarios.Any(u => u.Email == email)) return;

        context.Usuarios.Add(new Usuario("Usuário Teste", email, senha, EPerfilUsuario.Administrador, Guid.NewGuid())
        {
            
        });

        await context.SaveChangesAsync();
    }
}