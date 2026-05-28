using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Cliente.Shared
{
    public record EnderecoDTO
    {
        public string Logradouro { get; init; }
        public string Numero { get; init; }
        public string Complemento { get; init; }
        public string Bairro { get; init; }
        public string Cep { get; init; }
        public string Cidade { get; init; }
        public string Uf { get; init; }
    }
}
