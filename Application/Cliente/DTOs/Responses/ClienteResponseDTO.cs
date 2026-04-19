using Application.Cliente.DTOs.Shared;
using Domain.Aggregates.Cliente.ValueObjects;
using Domain.Entities;

namespace Application.Cliente.DTOs.Responses
{
    public record ClienteResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public ICollection<string> Emails { get; set; }
        public ICollection<TelefoneDTO> Telefones { get; set; }
        public ICollection<EnderecoDTO> Enderecos { get; set; }
    }
}
