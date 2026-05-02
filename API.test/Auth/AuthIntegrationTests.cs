using System.Net;
using System.Net.Http.Json;
using Application.Auth.DTOs.Requests;
using FluentAssertions;
using Integration.test.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Infra.Context;
using Domain.Entities;
using Xunit;

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
        // Arrange - Precisa de token de Admin para acessar este endpoint
        _fixture.AutenticarClient(Guid.NewGuid(), "Admin Global", "Admin");

        var request = new CriarUsuarioRequestDTO
        {
            Nome = "Novo Tecnico",
            Email = "tecnico@oficina.com",
            Senha = "Senha@123",
            Perfil = "Funcionario"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseRoute}/usuarios", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_CriarUsuarioSemToken_RetornaUnauthorized()
    {
        // Arrange - Garante que o client está sem cabeçalhos de autorização
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
        // Arrange - Mecânico não pode criar outros usuários (apenas Admin)
        _fixture.AutenticarClient(Guid.NewGuid(), "Mecanico Silva", "Funcionario");

        var request = new CriarUsuarioRequestDTO { /* dados */ };

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseRoute}/usuarios", request);

        // Assert - O ASP.NET retorna 403 quando o perfil não bate com o Role exigido
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // Helper para inserir usuário diretamente no banco via DbContext
    private async Task CriarUsuarioNoBancoAsync(string email, string senha)
    {
        // Nota: se o seu sistema usa BCrypt ou Identity, você deve salvar a senha hasheada aqui
        using var scope = _fixture.GetScope(); // Crie um método GetScope na sua Base se necessário
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Verifique se o usuário já existe para não quebrar a constraint de Unique
        if (context.Usuarios.Any(u => u.Email == email)) return;

        context.Usuarios.Add(new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Usuário Teste",
            Email = email,
            SenhaHash = senha, // Idealmente use seu hasher aqui
            Perfil = "Administrador"
        });

        await context.SaveChangesAsync();
    }
}