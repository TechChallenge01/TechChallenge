using Shared.DTOs.Cliente.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Cliente.Input
{
    public record ClienteInputDTO
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string? Cpf { get; init; }
        public string? Cnpj { get; init; }
        public string Email { get; init; }
        public TelefoneDTO Telefone { get; init; }
        public EnderecoDTO Endereco { get; init; }
        public List<Guid> Veiculos { get; init; }
    }
}
