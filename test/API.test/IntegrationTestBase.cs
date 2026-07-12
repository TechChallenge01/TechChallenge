using Application.Auth.DTOs.Requests;
using Infra.Context;
using Infra.DbModel;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace API.test;

public class IntegrationTestBase
{
    public async Task<AuthenticationHeaderValue> AuthenticateAsync(ApiWebApplicationFactory app, HttpClient client)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!context.Usuarios.Any(u => u.Email == "Admin@email.com"))
        {
            var admin = new UsuarioDbModel(
                Guid.NewGuid(), "Admin", "Admin@email.com",
                BCrypt.Net.BCrypt.HashPassword("12345678"),
                "Administrador",
                Guid.NewGuid(), DateTime.UtcNow, null, null, true);
            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();
        }

        var response = await client.PostAsJsonAsync("/api/login", new LoginRequestDTO { Email = "Admin@email.com", Senha = "12345678" });
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(content))
            throw new Exception($"Login falhou. Status: {response.StatusCode}. Body: '{content}'");

        // Case-insensitive: suporta tanto { "data": { "token": } } quanto { "Data": { "Token": } }
        var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        // Encontra "data" ou "Data"
        if (!root.TryGetProperty("data", out var dataEl) && !root.TryGetProperty("Data", out dataEl))
            throw new Exception($"Propriedade 'data' não encontrada na resposta de login. Body: '{content}'");

        // Encontra "token" ou "Token"
        if (!dataEl.TryGetProperty("token", out var tokenEl) && !dataEl.TryGetProperty("Token", out tokenEl))
            throw new Exception($"Propriedade 'token' não encontrada em 'data'. Body: '{content}'");

        var token = tokenEl.GetString();

        return new AuthenticationHeaderValue("Bearer", token);
    }
}