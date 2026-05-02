using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExtension
{
    public static Guid ObterIdUsuario(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
               ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var id))
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        return id;
    }

    public static string ObterPerfil(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
