using Application.Clientes.DTOs.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Clientes.DTOs.Requests
{
    public record ClienteRequestDTO
    {
        [Required]
        public string Nome { get; init; }
        public string Cpf { get; init; }
        public string Cnpj { get; init; }
        [Required]
        public string Email { get; init; }
        [Required]
        public TelefoneDTO Telefone { get; init; }
        public EnderecoDTO Enderecos { get; init; }
    }
}
