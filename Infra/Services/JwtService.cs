using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Auth.Services;

namespace Infra.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration configuration)
    {
        _config = configuration;
    }

    public (string token, DateTime expiracao) GerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiracao = DateTime.UtcNow.AddHours(int.Parse(_config["Jwt:ExpiracaoHoras"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name,               usuario.Nome),
            new Claim(ClaimTypes.Role,               usuario.Perfil.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiracao,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiracao);
    }
}
