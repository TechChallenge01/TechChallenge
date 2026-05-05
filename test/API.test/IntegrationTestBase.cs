using Application.Auth.DTOs.Requests;
using Domain.Entities;
using Domain.Enums;
using Infra.Context;
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
            var admin = new Usuario("Admin", "Admin@email.com", BCrypt.Net.BCrypt.HashPassword("12345678"), EPerfilUsuario.Administrador, Guid.NewGuid());
            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();
        }

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDTO { Email = "Admin@email.com", Senha = "12345678" });
        var content = await response.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(content).RootElement.GetProperty("data").GetProperty("token").GetString();

        return new AuthenticationHeaderValue("Bearer", token);
    }
}