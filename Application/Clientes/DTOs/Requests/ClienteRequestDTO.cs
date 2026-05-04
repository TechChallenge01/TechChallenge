using Application.Clientes.DTOs.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Clientes.DTOs.Requests
{
    public record ClienteRequestDTO
    {
        [Required]
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public TelefoneDTO Telefone { get; set; }
        public EnderecoDTO Enderecos { get; set; }
    }
}
