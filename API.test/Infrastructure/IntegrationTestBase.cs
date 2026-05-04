using Domain.Entities;
using Domain.Enums;
using Infra.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace API.test.Infrastructure;

public class IntegrationTestBase : WebApplicationFactory<Program>
{
    // Mesma chave usada no PostConfigure — todos os tokens gerados aqui serão aceitos pela API em memória
    public const string TestJwtKey = "chave-de-teste-minimo-32-caracteres-aqui!!";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext original e substitui pelo InMemory com nome único por instância
            // para evitar compartilhamento de estado entre classes de teste
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase($"TechChallengerTest_{Guid.NewGuid()}"));

            // Substitui a validação JWT para usar a chave de teste
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(TestJwtKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Cria o banco e aplica o seed
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            SeedDatabase(db);
        });
    }

    // Seed com usuário admin para testes de login
    private static void SeedDatabase(AppDbContext db)
    {
        if (db.Usuarios.Any()) return;

        db.Usuarios.Add(new Usuario("test", "admin@seed.com", BCrypt.Net.BCrypt.HashPassword("123456"), EPerfilUsuario.Administrador, Guid.Empty));

        db.SaveChanges();
    }

    // Cria um HttpClient com o token já configurado
    public HttpClient CriarClienteAutenticado(Guid idUsuario, string nome, string perfil)
    {
        var client = CreateClient();
        var token = GerarToken(idUsuario, nome, perfil);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    // Cria um HttpClient sem autenticação
    public HttpClient CriarClienteSemAutenticacao()
    {
        return CreateClient();
    }

    public static string GerarToken(Guid idUsuario, string nome, string perfil)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   idUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, $"{nome.ToLower().Replace(" ", "")}@teste.com"),
            new Claim(ClaimTypes.Name,               nome),
            new Claim(ClaimTypes.Role,               perfil),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}