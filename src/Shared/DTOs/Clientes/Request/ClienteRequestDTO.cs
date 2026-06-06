using Shared.DTOs.Clientes.Shared;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Clientes.Request
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
        public EnderecoDTO Endereco { get; init; }
        public ICollection<Guid> Veiculos { get; init; }
    }
}
