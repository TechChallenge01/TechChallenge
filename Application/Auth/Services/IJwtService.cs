using Domain.Entities;

namespace Application.Auth.Services;
public interface IJwtService
{
    (string token, DateTime expiracao) GerarToken(Usuario usuario);
}
