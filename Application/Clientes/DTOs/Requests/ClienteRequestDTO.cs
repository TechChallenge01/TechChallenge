using Application.Clientes.DTOs.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Clientes.DTOs.Requests
{
    public record ClienteRequestDTO
    {
        [Required]
        public string Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Cnpj { get; set; }
        [Required]
        public ICollection<string> Emails { get; set; }
        [Required]
        public ICollection<TelefoneDTO> Telefones { get; set; }
        public ICollection<EnderecoDTO> Enderecos { get; set; }
    }
}
