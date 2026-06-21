using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Usuarios.Output
{
    public record LoginOutputDTO
    {
        public string Token { get; init; }
        public string Nome { get; init; }
        public string Email { get; init; }
        public string Perfil { get; init; }
        public DateTime Expiracao { get; init; }
    }
}
