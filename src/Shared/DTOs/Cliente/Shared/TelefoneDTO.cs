using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Cliente.Shared
{
    public record TelefoneDTO
    {
        public string DDD { get; init; }
        public string DDI { get; init; }
        public string Numero { get; init; }
    }
}
