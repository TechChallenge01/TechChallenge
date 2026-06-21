using Domain.Entities;

namespace Application.Interfaces;
public interface IJwtService
{
    (string token, DateTime expiracao) GerarToken(Usuario usuario);
}
