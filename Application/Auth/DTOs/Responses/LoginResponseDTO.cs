using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.DTOs.Responses;

public record LoginResponseDTO
{
    public string Token { get; init; }
    public string Nome { get; init; }
    public string Email { get; init; }
    public string Perfil { get; init; }
    public DateTime Expiracao { get; init; }
}
